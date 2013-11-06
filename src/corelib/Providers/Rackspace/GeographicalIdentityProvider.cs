using System;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    internal class GeographicalIdentityProvider : IExtendedIdentityProvider
    {
        private readonly IRestService _restService;
        private readonly ICache<UserAccess> _userAccessCache;
        private readonly Uri _urlBase;

        public GeographicalIdentityProvider(Uri urlBase, IRestService restService, ICache<UserAccess> userAccessCache)
        {
            _urlBase = urlBase;
            _userAccessCache = userAccessCache;
            _restService = restService;
        }

        #region Roles

        public Role[] ListRoles(CloudIdentity identity, string serviceId = null, int? marker = null, int? limit = 10000)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");

            var parameters = BuildOptionalParameterList(new Dictionary<string, string>
                {
                    {"serviceId", serviceId},
                    {"marker", !marker.HasValue ? null : marker.Value.ToString()},
                    {"limit", !limit.HasValue ? null : limit.Value.ToString()},
                });

            var response = ExecuteRESTRequest<RolesResponse>(identity, "/v2.0/OS-KSADM/roles", HttpMethod.GET, queryStringParameter: parameters);
            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public Role AddRole(CloudIdentity identity, Role role)
        {
            var response = ExecuteRESTRequest<RoleResponse>(identity, "/v2.0/OS-KSADM/roles", HttpMethod.POST, new AddRoleRequest{Role = role});

            if (response == null || response.Data == null)
                return null;

            return response.Data.Role;
        }

        public Role GetRole(CloudIdentity identity, string roleId)
        {
            var urlPath = string.Format("/v2.0/OS-KSADM/roles/{0}", roleId);
            var response = ExecuteRESTRequest<RoleResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Role;
        }

        public Role[] GetRolesByUser(CloudIdentity identity, string userId)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles", userId);
            var response = ExecuteRESTRequest<RolesResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public bool AddRoleToUser(CloudIdentity identity, string userId, string roleId)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || (response.StatusCode >= 400 && response.StatusCode != 409))
                return false;

            return true;
        }

        public bool DeleteRoleFromUser(CloudIdentity identity, string userId, string roleId)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response != null && response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Credentials

        public bool SetUserPassword(CloudIdentity identity, string userId, string password)
        {
            var user = GetUser(identity, userId);

            return SetUserPassword(identity, user, password);
        }

        public bool SetUserPassword(CloudIdentity identity, User user, string password)
        {
            return SetUserPassword(identity, user.Id, user.Username, password);
        }

        public bool SetUserPassword(CloudIdentity identity, string userId, string username, string password)
        {
            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials", userId);
            var request = new SetPasswordRequest
            {
                PasswordCredencial =
                    new PasswordCredencial { Username = username, Password = password }
            };
            var response = ExecuteRESTRequest<PasswordCredencialResponse>(identity, urlPath, HttpMethod.POST, request);

            if (response == null || response.StatusCode != 201 || response.Data == null)
                return false;

            return response.Data.PasswordCredencial.Password.Equals(password);
        }

        public UserCredential[] ListUserCredentials(CloudIdentity identity, string userId)
        {
            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials", userId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.GET);

            if (response == null || string.IsNullOrWhiteSpace(response.RawBody))
                return null;

            var jObject = JObject.Parse(response.RawBody);
            var credsArray = (JArray)jObject["credentials"];
            var creds = new List<UserCredential>();

            foreach (JObject jToken in credsArray)
            {
                foreach (JProperty property in jToken.Properties())
                {
                    var cred = (JObject)property.Value;
                    creds.Add(new UserCredential
                            {
                                Name = property.Name,
                                APIKey = cred["apiKey"].ToString().Replace("\"", string.Empty),
                                Username = cred["username"].ToString().Replace("\"", string.Empty),
                            }); 
                }
                   
            }
            
            return creds.ToArray();
        }

        public UserCredential GetUserCredential(CloudIdentity identity, string userId, string credentialKey)
        {
            var creds = ListUserCredentials(identity, userId);

            var cred = creds.FirstOrDefault(c => c.Name.Equals(credentialKey, StringComparison.OrdinalIgnoreCase));

            return cred;
        }

        public UserCredential UpdateUserCredentials(CloudIdentity identity, string userId, string apiKey)
        {
            var user = GetUser(identity, userId);

            return UpdateUserCredentials(identity, user, apiKey);
        }

        public UserCredential UpdateUserCredentials(CloudIdentity identity, User user, string apiKey)
        {
            return UpdateUserCredentials(identity, user.Id, user.Username, apiKey);
        }

        public UserCredential UpdateUserCredentials(CloudIdentity identity, string userId, string username, string apiKey)
        {
            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials/RAX-KSKEY:apiKeyCredentials", userId);
            var request = new UpdateUserCredencialRequest { UserCredential = new UserCredential { Username = username, APIKey = apiKey } };
            var response = ExecuteRESTRequest<UserCredentialResponse>(identity, urlPath, HttpMethod.POST, request);

            if (response == null || response.Data == null)
                return null;

            return response.Data.UserCredential;
        }

        public bool DeleteUserCredentials(CloudIdentity identity, string userId)
        {
            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials/RAX-KSKEY:apiKeyCredentials", userId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response == null || response.StatusCode != 200)
                return false;

            return true;
        }

        #endregion

        #region Users

        public User[] ListUsers(CloudIdentity identity)
        {
            var response = ExecuteRESTRequest<UsersResponse>(identity, "/v2.0/users", HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            // Due to the fact the sometimes the API returns a JSON array of users and sometimes it returns a single JSON user object.  
            // Therefore if we get a null data object (which indicates that the deserializer could not parse to an array) we need to try and parse as a single User object.
            if(response.Data.Users == null)
            {
                var userResponse = JsonConvert.DeserializeObject<UserResponse>(response.RawBody);

                if (response == null || response.Data == null)
                    return null;

                return new[] {userResponse.User};
            }

            return response.Data.Users;
        }

        public User GetUserByName(CloudIdentity identity, string name)
        {
            var urlPath = string.Format("/v2.0/users/?name={0}", name);
            var response = ExecuteRESTRequest<UserResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.User;
        }

        public User GetUser(CloudIdentity identity, string userId)
        {
            var urlPath = string.Format("v2.0/users/{0}", userId);

            var response = ExecuteRESTRequest<UserResponse>(identity, urlPath, HttpMethod.GET);

            return response.Data.User;
        }

        public NewUser AddUser(CloudIdentity identity, NewUser newUser)
        {
            newUser.Id = null;

            var response = ExecuteRESTRequest<NewUserResponse>(identity, "/v2.0/users", HttpMethod.POST, new AddUserRequest { User = newUser });

            if (response == null || response.Data == null)
                return null;

            // If the user specifies a password, then the password will not be in the response, so we need to fill it in on the return object.
            if (string.IsNullOrWhiteSpace(response.Data.NewUser.Password))
                response.Data.NewUser.Password = newUser.Password;

            return response.Data.NewUser;
        }

        public User UpdateUser(CloudIdentity identity, User user)
        {
            if(user == null || string.IsNullOrWhiteSpace(user.Id))
                throw new ArgumentException("The User or User.Id values cannot be null.");

            var urlPath = string.Format("v2.0/users/{0}", user.Id);

            var updateUserRequest = new UpdateUserRequest { User = user };
            var response = ExecuteRESTRequest<UserResponse>(identity, urlPath, HttpMethod.POST, updateUserRequest);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || response.Data == null || (response.StatusCode >= 400 && response.StatusCode != 409))
                return null;

            return response.Data.User;
        }

        public bool DeleteUser(CloudIdentity identity, string userId)
        {
            var urlPath = string.Format("/v2.0/users/{0}", userId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response != null && response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Tenants

        public Tenant[] ListTenants(CloudIdentity identity)
        {
            var response = ExecuteRESTRequest<TenantsResponse>(identity, "v2.0/tenants", HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Tenants;
        }

        #endregion

        #region Token and Authentication

        public string GetToken(CloudIdentity idenity, bool forceCacheRefresh = false)
        {
            var auth = GetUserAccess(idenity, forceCacheRefresh);

            if (auth == null || auth.Token == null)
                return null;

            return auth.Token.Id;
        }

        public string GetToken(RackspaceImpersonationIdentity identity, bool forceCacheRefresh = false)
        {
            var auth = GetUserAccess(identity, forceCacheRefresh);

            if (auth == null || auth.Token == null)
                return null;

            return auth.Token.Id;
        }

        public IdentityToken GetTokenInfo(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var auth = GetUserAccess(identity, forceCacheRefresh);

            if (auth == null)
                return null;

            return auth.Token;
        }

        public UserAccess Authenticate(CloudIdentity identity)
        {
            return GetUserAccess(identity, true);
        }

        public UserAccess GetUserAccess(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var cloudInstance = CloudInstance.Default;
            var rackspaceCloudIdentity = identity as RackspaceCloudIdentity;

            if (rackspaceCloudIdentity != null)
                cloudInstance = rackspaceCloudIdentity.CloudInstance;
            var userAccess = _userAccessCache.Get(string.Format("{0}/{1}", cloudInstance, identity.Username), () =>
                                                    {
                                                        var auth = AuthRequest.FromCloudIdentity(identity);
                                                        var response = ExecuteRESTRequest<AuthenticationResponse>(identity, "/v2.0/tokens", HttpMethod.POST, auth, isTokenRequest: true);


                                                        if (response == null || response.Data == null || response.Data.UserAccess == null || response.Data.UserAccess.Token == null)
                                                            return null;

                                                        return response.Data.UserAccess;
                                                    }, forceCacheRefresh);

            return userAccess;
        }

        public UserAccess Authenticate(RackspaceImpersonationIdentity identity, bool forceCacheRefresh = false)
        {
            var impToken = _userAccessCache.Get(string.Format("imp/{0}/{1}", identity.UserToImpersonate.CloudInstance, identity.UserToImpersonate.Username), () => {
                const string urlPath = "/v2.0/RAX-AUTH/impersonation-tokens";
                var request = BuildImpersonationRequestJson(urlPath, identity.UserToImpersonate.Username, 600);
                var response = ExecuteRESTRequest<UserImpersonationResponse>(identity, urlPath, HttpMethod.POST, request);

                if (response == null || response.Data == null || response.Data.UserAccess == null)
                    return null;

                return response.Data.UserAccess;
            }, forceCacheRefresh);

            return impToken;
        }

        private JObject BuildImpersonationRequestJson(string path, string userName, int expirationInSeconds)
        {
            var request = new JObject();
            var impInfo = new JObject();
            var user = new JObject { { "username", userName }, { "expire-in-seconds", expirationInSeconds } };
            impInfo.Add("user", user);
            var parts = path.Split('/');
            request.Add(string.Format("{0}:impersonation", parts[1]), impInfo);

            return request;
        }

        #endregion

        protected virtual Response ExecuteRESTRequest(CloudIdentity identity, string urlPath, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null, bool isTokenRequest = false, string token = null, int retryCount = 2, int retryDelay = 200)
        {
            return ExecuteRESTRequest<object>(identity, urlPath, method, body, queryStringParameter, false, isTokenRequest, token, retryCount, retryDelay);
        }

        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, string urlPath, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null,  bool isRetry = false, bool isTokenRequest = false, string token = null, int retryCount = 2, int retryDelay = 200) where T : new()
        {
            var url = new Uri(_urlBase, urlPath);

            var headers = new Dictionary<string, string>();

            if (!isTokenRequest)
                headers.Add("X-Auth-Token", string.IsNullOrWhiteSpace(token) ? GetToken(identity) : token);

            string bodyStr = null;
            if (body != null)
            {
                if (body is JObject)
                    bodyStr = body.ToString();
                else
                    bodyStr = JsonConvert.SerializeObject(body, Formatting.None, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});
            }

            var response = _restService.Execute<T>(url, method, bodyStr, headers, queryStringParameter, new JsonRequestSettings() { RetryCount = retryCount, RetryDelayInMS = retryDelay, Non200SuccessCodes = new[] { 401, 409 }, UserAgent = ProviderBase.GetUserAgentHeaderValue()});

            // on errors try again 1 time.
            if (response.StatusCode == 401 && !isRetry && !isTokenRequest)
            {
                return ExecuteRESTRequest<T>(identity,urlPath, method, body, queryStringParameter, true, isTokenRequest, GetToken(identity));
            }

            ProviderBase.CheckResponse(response);

            return response;
        }

        protected Dictionary<string, string> BuildOptionalParameterList(Dictionary<string, string> optionalParameters)
        {
            if (optionalParameters == null)
                return null;

            var paramList = optionalParameters.Where(optionalParameter => !string.IsNullOrEmpty(optionalParameter.Value)).ToDictionary(optionalParameter => optionalParameter.Key, optionalParameter => optionalParameter.Value, optionalParameters.Comparer);
            if (!paramList.Any())
                return null;

            return paramList;
        }
    }
}
