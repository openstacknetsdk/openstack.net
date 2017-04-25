using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace OpenStack.Authentication.V3.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityApi : IAuthenticationProvider
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  Task<string> GetToken(CancellationToken cancellationToken)
        {
            return new Task<string>(() =>
           {
               return this.GetToken(this._identityEndpoint.ToString(), this._authRequest, cancellationToken).Id;
           });
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
            return this._tokenCache.Get(key, () =>  
                {
                    PreparedRequest request = BuildGetTokenRequest(identityUrl, authRequest, cancellationToken);
                    var t = request
                        .SendAsync();
                    var resp = t.Result;
                    string content = resp.Content.ToString();
                    AuthResponse authResp = JsonConvert.DeserializeObject<AuthResponse>(content);
                    Token token = authResp.Token;
                    IEnumerable<string> identities;
                    resp.Headers.TryGetValues(" X-Subject-Token", out identities);
                    token.Id = identities.First();
                    return token;
                }, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public string GetCacheTokenKey()
        {
            string key;
            if (this._credential.User.Id != null)
            {
                key = string.Format("{0}:{1}", this._identityEndpoint.ToString(), this._credential.User.Id);
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
