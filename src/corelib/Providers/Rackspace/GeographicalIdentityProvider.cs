using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleRestServices.Client;
using SimpleRestServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Objects;

namespace net.openstack.Providers.Rackspace
{
    internal class GeographicalIdentityProvider : IIdentityProvider
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
        
        public Role[] ListRoles(CloudIdentity identity)
        {
            var response = ExecuteRESTRequest<RolesResponse>("/v2.0/OS-KSADM/roles", HttpMethod.GET, null, identity);
            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public Role[] GetRolesByUser(string userId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles", userId);
            var response = ExecuteRESTRequest<RolesResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.PUT, null, identity);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || (response.StatusCode >= 400 && response.StatusCode != 409))
                return false;

            return true;
        }

        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity);

            if (response != null && response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Users
        
        public User GetUserByName(string name, CloudIdentity identity)
        {
            var urlPath = string.Format("/v2.0/users/?name={0}", name);
            var response = ExecuteRESTRequest<UserResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.User;
        }
        #endregion

        public string GetToken(CloudIdentity idenity)
        {
            var auth = Authenticate(idenity);

            if (auth == null || auth.Token == null)
                return null;

            return auth.Token.Id;
        }

        public string GetToken(ImpersonationIdentity identity)
        {
            var auth = Authenticate(identity);

            if (auth == null || auth.Token == null)
                return null;

            return auth.Token.Id;
        }

        public IdentityToken GetTokenInfo(CloudIdentity identity)
        {
            var auth = Authenticate(identity);

            if (auth == null)
                return null;

            return auth.Token;
        }

        public UserAccess Authenticate(CloudIdentity identity)
        {
            var userAccess = _userAccessCache.Get(string.Format("{0}/{1}", identity.Region, identity.Username), () => {
                var auth = AuthRequest.FromCloudIdentity(identity);
                var response = ExecuteRESTRequest<AuthenticationResponse>("/v2.0/tokens", HttpMethod.POST, auth, identity, isTokenRequest: true);


                if (response == null || response.Data == null || response.Data.UserAccess == null || response.Data.UserAccess.Token == null)
                    return null;

                return response.Data.UserAccess;
            });

            return userAccess;
        }

        public UserAccess Authenticate(ImpersonationIdentity identity)
        {
            var impToken = _userAccessCache.Get(string.Format("imp/{0}/{1}", identity.UserToImpersonate.Region, identity.UserToImpersonate.Username), () => {
                const string urlPath = "/v2.0/RAX-AUTH/impersonation-tokens";
                var request = BuildImpersonationRequestJson(urlPath, identity.UserToImpersonate.Username, 600);
                var response = ExecuteRESTRequest<UserImpersonationResponse>(urlPath, HttpMethod.POST, request, identity);

                if (response == null || response.Data == null || response.Data.UserAccess == null)
                    return null;

                return response.Data.UserAccess;
            });

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

        protected virtual bool ExecuteRESTRequest(string urlPath, HttpMethod method, object body, CloudIdentity identity, bool isTokenRequest = false, string token = null, int retryCount = 2, int retryDelay = 200)
        {
            var response = ExecuteRESTRequest<object>(urlPath, method, body, identity, false, isTokenRequest, token, retryCount, retryDelay);

            if (response == null)
                return false;

            if (response.StatusCode >= 400)
                return false;

            return true;
        }

        protected Response<T> ExecuteRESTRequest<T>(string urlPath, HttpMethod method, object body, CloudIdentity identity, bool isRetry = false, bool isTokenRequest = false, string token = null, int retryCount = 2, int retryDelay = 200) where T : new()
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
                    bodyStr = JsonConvert.SerializeObject(body, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});
            }

            var response = _restService.Execute<T>(url, method, bodyStr, headers, new JsonRequestSettings() { RetryCount = retryCount, RetryDelayInMS = retryDelay, Non200SuccessCodes = new[] { 401, 409 } });

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    // if authentication failed, assume the token ran out and refresh.
                    //if (response.StatusCode == 401 && !isTokenRequest)
                    //    GetRackConnectToken(region, modifiedBy, true);

                    return ExecuteRESTRequest<T>(urlPath, method, body, identity, true, isTokenRequest, GetToken(identity));
                }
            }

            return response;
        }
    }
}
