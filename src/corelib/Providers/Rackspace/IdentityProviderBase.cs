using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using net.openstack.corelib.Providers.Rackspace.Objects;
using net.openstack.corelib.Web;

namespace net.openstack.corelib.Providers.Rackspace
{
    public abstract class IdentityProviderBase
    {
        protected IdentityProviderBase(Uri urlBase, IRestService restService, ICache<IdentityToken> tokenCache)
        {
            URLBase = urlBase;
            _tokenCache = tokenCache;
            RestService = restService;
        }

        #region Private Instance Members
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

        public IdentityToken GetTokenInfo(CloudIdentity identity)
        {
            
            var auth = identity.BuildAuthRequestJson();
            var response = ExecuteRESTRequest<TokenResponse>("v2.0/tokens", HttpMethod.POST, auth, identity, isTokenRequest: true);


            if (response == null || response.Data == null || response.Data.access == null || response.Data.access.token == null)
                return null;

            return new IdentityToken()
                {
                    Id = response.Data.access.token.id,
                    Expires = response.Data.access.token.expires
                }; ;
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

        protected WebResponse<T> ExecuteRESTRequest<T>(string urlPath, HttpMethod method, object body, CloudIdentity identity, bool isRetry = false, bool isTokenRequest = false, string token = null, int retryCount = 2, int retryDelay = 200) where T : new()
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
                    bodyStr = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            }

            var response = RestService.Execute<T>(url, method, bodyStr, headers, retryCount: retryCount, delayInMilliseconds: retryDelay, non200SuccessCodes: new[] { 401, 409 });

            // on errors try again 1 time.
            if (response.StatusCode == 401)
            {
                if (!isRetry)
                {
                    // if authentication failed, assume the token ran out and refresh.
                    //if (response.StatusCode == 401 && !isTokenRequest)
                    //    GetRackConnectToken(region, modifiedBy, true);

                    return ExecuteRESTRequest<T>(urlPath, method, body, identity, true, isTokenRequest, token);
                }
            }

            return response;
        }

        #endregion
    }
}
