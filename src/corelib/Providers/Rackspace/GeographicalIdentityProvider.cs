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
        public GeographicalIdentityProvider(Uri urlBase)
            : this(urlBase, new JsonRestServices(), new IdentityTokenCache())
        {
        }
        public GeographicalIdentityProvider(Uri urlBase, IRestService restService, ICache<IdentityToken> tokenCache)
        {
            URLBase = urlBase;
            _tokenCache = tokenCache;
            RestService = restService;
        }

        #region Roles
        
        public Role[] ListRoles(CloudIdentity identity)
        {
            const string urlPath = Settings.GetAllRolesUrlFormat;
            var response = ExecuteRESTRequest<RolesResponse>(urlPath, HttpMethod.GET, null, identity);
            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public Role[] GetRolesByUser(string userId, CloudIdentity identity)
        {
            const string urlFormat = Settings.GetRolesByUserUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "userID", userId } });
            var response = ExecuteRESTRequest<RolesResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            const string urlFormat = Settings.AddRoleToUserUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "userID", userId }, { "roleID", roleId } });
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.PUT, null, identity);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || (response.StatusCode >= 400 && response.StatusCode != 409))
                return false;

            return true;
        }

        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            const string urlFormat = Settings.DeleteRoleFromUserUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "userID", userId }, { "roleID", roleId } });
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity);

            if (response != null && response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Users
        
        public User GetUserByName(string name, CloudIdentity identity)
        {
            const string urlFormat = Settings.GetUserByNameUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "name", name } });
            var response = ExecuteRESTRequest<UserResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.User;
        }
        #endregion
        
        protected readonly IRestService RestService;
        private readonly ICache<IdentityToken> _tokenCache;
        protected readonly Uri URLBase;

        public string GetToken(CloudIdentity idenity)
        {
            var token = _tokenCache.Get(string.Format("{0}/{1}", idenity.Region, idenity.Username), () => GetTokenInfo(idenity));
            
            if (token == null)
                return null;

            return token.Id;
        }

        public string GetToken(ImpersonationIdentity idenity)
        {
            var token = _tokenCache.Get(string.Format("{0}/{1}", idenity.Region, idenity.Username), () => GetTokenInfo(idenity));

            if (token == null)
                return null;

            var impToken = _tokenCache.Get(string.Format("imp/{0}/{1}", idenity.UserToImpersonate.Region, idenity.UserToImpersonate.Username),
                    () => GetUserImpersonationToken(idenity.UserToImpersonate.Username, idenity) );

            if (impToken == null)
                return null;

            return impToken.Id;
        }

        public IdentityToken GetTokenInfo(CloudIdentity identity)
        {

            var auth = AuthRequest.FromCloudIdentity(identity);
            var response = ExecuteRESTRequest<TokenResponse>(Settings.GetTokenInfoUrlFormat, HttpMethod.POST, auth, identity, isTokenRequest: true);


            if (response == null || response.Data == null || response.Data.Access == null || response.Data.Access.Token == null)
                return null;

            return response.Data.Access.Token;
        }

        public IdentityToken GetUserImpersonationToken(string userName, CloudIdentity identity)
        {
            const string urlPath = Settings.GetUserImpersonationTokenUrlFormat;
            var request = BuildImpersonationRequestJson(urlPath, userName, 600);
            var response = ExecuteRESTRequest<UserImpersonationResponse>(urlPath, HttpMethod.POST, request, identity);

            if (response == null || response.Data == null || response.Data.Access == null || response.Data.Access.Token == null)
                return null;

            return response.Data.Access.Token;
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
            var url = new Uri(URLBase, urlPath);

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

            var response = RestService.Execute<T>(url, method, bodyStr, headers, new JsonRequestSettings() { RetryCount = retryCount, RetryDelayInMS = retryDelay, Non200SuccessCodes = new[] { 401, 409 } });

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
