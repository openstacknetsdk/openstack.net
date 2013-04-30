using System;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using net.openstack.Core;
using net.openstack.Core.Caching;
using net.openstack.Core.Domain;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    internal class GeographicalCloudIdentityProvider : IExtendedCloudIdentityProvider
    {
        private readonly IRestService _restService;
        private readonly ICache<UserAccess> _userAccessCache;
        private readonly Uri _urlBase;
        private readonly CloudIdentity _defaultIdentity;
        private readonly IHttpResponseCodeValidator _responseCodeValidator;

        public GeographicalCloudIdentityProvider(Uri urlBase, CloudIdentity identity, IRestService restService, ICache<UserAccess> userAccessCache, IHttpResponseCodeValidator responseCodeValidator)
        {
            _defaultIdentity = identity;
            _urlBase = urlBase;
            _userAccessCache = userAccessCache;
            _restService = restService;
            _responseCodeValidator = responseCodeValidator;
        }

        #region Roles

        public IEnumerable<Role> ListRoles(CloudIdentity identity)
        {
            var response = ExecuteRESTRequest<RolesResponse>(identity, "/v2.0/OS-KSADM/roles",
                                                                             HttpMethod.GET);
            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public Role AddRole(Role role, CloudIdentity identity)
        {
            var response = ExecuteRESTRequest<RoleResponse>(identity, "/v2.0/OS-KSADM/roles", HttpMethod.POST, new AddRoleRequest{Role = role});

            if (response == null || response.Data == null)
                return null;

            return response.Data.Role;
        }

        public Role GetRole(string roleId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/OS-KSADM/roles/{0}", roleId);
            var response = ExecuteRESTRequest<RoleResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Role;
        }

        public IEnumerable<Role> GetRolesByUser(string userId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles", userId);
            var response = ExecuteRESTRequest<RolesResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || (response.StatusCode >= 400 && response.StatusCode != 409))
                return false;

            return true;
        }

        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response != null && response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Credentials

        public bool SetUserPassword(string userId, string password, CloudIdentity identity)
        {
            var user = GetUser(userId, identity);

            return SetUserPassword(user, password, identity);
        }

        public bool SetUserPassword(User user, string password, CloudIdentity identity)
        {
            return SetUserPassword(user.Id, user.Username, password, identity);
        }

        public bool SetUserPassword(string userId, string username, string password, CloudIdentity identity)
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

        public IEnumerable<UserCredential> ListUserCredentials(string userId, CloudIdentity identity)
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
                                APIKey = cred["apiKey"].ToString(),
                                Username = cred["username"].ToString()
                            }); 
                }
                   
            }
            
            return creds.ToArray();
        }

        public UserCredential GetUserCredential(string userId, string credentialKey, CloudIdentity identity)
        {
            var creds = ListUserCredentials(userId, identity);

            var cred = creds.FirstOrDefault(c => c.Name.Equals(credentialKey, StringComparison.OrdinalIgnoreCase));

            return cred;
        }

        public CloudIdentity DefaultIdentity { get { return _defaultIdentity;  } }

        public UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity)
        {
            var user = GetUser(userId, identity);

            return UpdateUserCredentials(user, apiKey, identity);
        }

        public UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity)
        {
            return UpdateUserCredentials(user.Id, user.Username, apiKey, identity);
        }

        public UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity)
        {
            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials/RAX-KSKEY:apiKeyCredentials", userId);
            var request = new UpdateUserCredencialRequest { UserCredential = new UserCredential { Username = username, APIKey = apiKey } };
            var response = ExecuteRESTRequest<UserCredentialResponse>(identity, urlPath, HttpMethod.POST, request);

            if (response == null || response.Data == null)
                return null;

            return response.Data.UserCredential;
        }

        public bool DeleteUserCredentials(string userId, CloudIdentity identity)
        {
            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials/RAX-KSKEY:apiKeyCredentials", userId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response == null || response.StatusCode != 200)
                return false;

            return true;
        }

        #endregion

        #region Users

        public IEnumerable<User> ListUsers(CloudIdentity identity)
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

        public User GetUserByName(string name, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/?name={0}", name);
            var response = ExecuteRESTRequest<UserResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.User;
        }

        public User GetUser(string userId, CloudIdentity identity)
        {
            var urlPath = string.Format("v2.0/users/{0}", userId);

            var response = ExecuteRESTRequest<UserResponse>(identity, urlPath, HttpMethod.GET);

            return response.Data.User;
        }

        public NewUser AddUser(NewUser newUser, CloudIdentity identity)
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

        public User UpdateUser(User user, CloudIdentity identity)
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

        public bool DeleteUser(string userId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}", userId);
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response != null && response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Tenants

        public IEnumerable<Tenant> ListTenants(CloudIdentity identity)
        {
            var response = ExecuteRESTRequest<TenantsResponse>(identity, "v2.0/tenants", HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Tenants;
        }

        #endregion

        #region Token and Authentication

        public string GetToken(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var auth = GetUserAccess(identity, forceCacheRefresh);

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
            if (identity == null)
                identity = _defaultIdentity;

            var rackspaceCloudIdentity = identity as RackspaceCloudIdentity;

            if (rackspaceCloudIdentity == null)
                rackspaceCloudIdentity = new RackspaceCloudIdentity(identity);

            var userAccess = _userAccessCache.Get(string.Format("{0}/{1}", rackspaceCloudIdentity.CloudInstance, rackspaceCloudIdentity.Username), () =>
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
            return ExecuteRESTRequest<Response>(identity, urlPath, method, body, queryStringParameter, false, isTokenRequest, token, retryCount, retryDelay,
                (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => _restService.Execute(uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings));
        }

        protected Response<T> ExecuteRESTRequest<T>(CloudIdentity identity, string urlPath, HttpMethod method, object body = null, Dictionary<string, string> queryStringParameter = null,  bool isRetry = false, bool isTokenRequest = false, string token = null, int retryCount = 2, int retryDelay = 200) where T : new()
        {
            return ExecuteRESTRequest<Response<T>>(identity, urlPath, method, body, queryStringParameter, false, isTokenRequest, token, retryCount, retryDelay,
                (uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings) => _restService.Execute<T>(uri, requestMethod, requestBody, requestHeaders, requestQueryParams, requestSettings));

        }

        protected T ExecuteRESTRequest<T>(CloudIdentity identity, string urlPath, HttpMethod method, object body, Dictionary<string, string> queryStringParameter, bool isRetry, bool isTokenRequest, string token, int retryCount, int retryDelay, 
            Func<Uri, HttpMethod, string, Dictionary<string, string>, Dictionary<string, string>, JsonRequestSettings, T> callback) where T : Response
        {
            if (identity == null)
                identity = _defaultIdentity;

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
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            var settings = new JsonRequestSettings()
                               {
                                   RetryCount = retryCount,
                                   RetryDelayInMS = retryDelay,
                                   Non200SuccessCodes = new[] {401, 409},
                                   UserAgent = UserAgentGenerator.Generate()
                               };

            var response = callback(url, method, bodyStr, headers, queryStringParameter, settings);

            // on errors try again 1 time.
            if (response.StatusCode == 401 && !isRetry && !isTokenRequest)
            {
                return ExecuteRESTRequest<T>(identity, urlPath, method, body, queryStringParameter, true, isTokenRequest, GetToken(identity), retryCount, retryCount, callback);
            }

            _responseCodeValidator.Validate(response);

            return response;
        }
    }
}
