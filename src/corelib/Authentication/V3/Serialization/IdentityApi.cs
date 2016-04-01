using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;

namespace OpenStack.Authentication.V3.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    internal class IdentityApi : IAuthenticationProvider
    {
        private Credential _credential;
        private Uri _identityEndpoint;
        private TokenCache _tokenCache;
        private AuthRequest _authRequest;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityEndpoint"></param>
        /// <param name="credential"></param>
        public IdentityApi(Uri identityEndpoint, Credential credential)
        {
            this._identityEndpoint = identityEndpoint; 
            this._credential = credential;
            this._authRequest = new AuthRequest(this._credential);
            this._tokenCache = new TokenCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="region"></param>
        /// <param name="useInternalUrl"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetEndpoint(IServiceType serviceType, string region, bool useInternalUrl, CancellationToken cancellationToken)
        {
            var token = this.GetToken(this.GetTokenUrl(), this._authRequest, cancellationToken).Catalog;
            Func<string> getEndpoint = () =>
            {
                foreach (var catalog in token.Where(catalog => catalog.Type == serviceType.Type))
                {
                    return catalog.Endpoints.Where(endpoint =>
                        {

                            return string.Equals(endpoint.Region, region, StringComparison.OrdinalIgnoreCase) &&
                                (useInternalUrl ? string.Equals(endpoint.Interface, "internal", StringComparison.OrdinalIgnoreCase) :
                                    string.Equals(endpoint.Interface, "public", StringComparison.OrdinalIgnoreCase));
                        }).First<Endpoint>().Url;
                }
                return null;
            };
            return Task.Factory.StartNew(getEndpoint);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  Task<string> GetToken(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew( () =>
           {
               return this.GetToken(this.GetTokenUrl(), this._authRequest, cancellationToken).Id;
           });
         }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetTokenUrl()
        {
            return string.Format("{0}://{1}:{2}/v3/auth/tokens", this._identityEndpoint.Scheme, this._identityEndpoint.Host, this._identityEndpoint.Port);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityUrl"></param>
        /// <param name="authRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Token GetToken(string identityUrl, AuthRequest authRequest, CancellationToken cancellationToken)
        {
            string key = this.GetCacheTokenKey();
            Func<Token> refreshCallback =
                () =>
                {
                    string content = JsonConvert.SerializeObject(this._authRequest, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(identityUrl);
                    var data = Encoding.ASCII.GetBytes(content);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    AuthResponse resp = JsonConvert.DeserializeObject<AuthResponse>(responseString);
                    resp.Token.SetIdentity(response.Headers.Get("X-Subject-Token"));
                    return resp.Token;
                };
            return this._tokenCache.Get(key, refreshCallback, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetCacheTokenKey()
        {
            string key;
            if (this._credential.passwordIdentity.password["user"].Id != null)
            {
                key = string.Format("{0}:{1}", this._identityEndpoint.ToString(), this._credential.passwordIdentity.password["user"].Id);
                return key;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityUrl"></param>
        /// <param name="authRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public PreparedRequest BuildGetTokenRequest(string identityUrl, AuthRequest authRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            PreparedRequest request = new PreparedRequest(identityUrl);
            return request.PreparePostJson(authRequest, cancellationToken);
        }
    }
}
