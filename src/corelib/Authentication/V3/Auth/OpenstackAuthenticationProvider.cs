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
using OpenStack.Serialization;
using OpenStack.Authentication.V3.Serialization;

namespace OpenStack.Authentication.V3.Auth
{
    /// <summary>
    ///Support all identity service since v3.1
    /// </summary>
    public class OpenstackAuthenticationProvider : IAuthenticationProvider
    {
        private readonly UserCredential _userCredential;
        private TokenCache _tokenCache;

        /// <summary>
        /// the baseurl of identity server 
        /// </summary>
        public readonly Uri Endpoint;

        /// <summary>
        /// represent user to be used for authentication
        /// </summary>
        public readonly AuthUser User;

        /// <summary>
        ///represent authentication, project or domain 
        /// </summary>
        public readonly AuthScope Scope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="scopeId"></param>
        /// <param name="scopType"></param>
        public OpenstackAuthenticationProvider(string endpoint, string userId, string password, string scopeId = null, AuthScopeType scopType = null)
        {

            User = new AuthUser(userId, password);
            if(scopeId !=null & scopType !=null)
                Scope = new AuthScope(scopeId, scopType);
            Endpoint = new Uri(endpoint);
            _userCredential = new UserCredential(User, Scope);
            _tokenCache = new TokenCache();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="region"></param>
        /// <param name="useInternalUrl"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetEndpoint(IServiceType serviceType, string region, bool useInternalUrl, CancellationToken cancellationToken)
        {
            var tokenCache = await GetTokenCache(cancellationToken);
            foreach (var catalog in tokenCache.Catalog.Where(catalog => catalog.Type == serviceType.Type))
            {
                return catalog.Endpoints.Where(endpoint =>
                    {
                        return string.Equals(endpoint.Region, region, StringComparison.OrdinalIgnoreCase) &&
                            (useInternalUrl ? string.Equals(endpoint.Interface, "internal", StringComparison.OrdinalIgnoreCase) :
                                string.Equals(endpoint.Interface, "public", StringComparison.OrdinalIgnoreCase));
                    }).First<Endpoint>().Url;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetToken(CancellationToken cancellationToken)
        {
           var token = await GetTokenCache(cancellationToken);
            return token.Id;
         }

        /// <summary>
        ///Get a token object from cach
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Token> GetTokenCache(CancellationToken cancellationToken)
        {
            return Task<Token>.Factory.StartNew(() =>
           {
               var key = BuildCacheTokenKey();
               var authRequest = new AuthRequest(_userCredential);
               return _tokenCache.Get(key, () =>
                {
                    var respMsg = new PreparedRequest(BuildIdentityUrl().ToString(), true)
                        .PreparePostJsonIgonreNull(authRequest)
                        .SendAsync()
                        .Result;
                    var content = respMsg.Content.ReadAsStringAsync().Result;
                    var id = respMsg.Headers.GetValues("X-Subject-Token").First<string>();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(content);
                    authResponse.Token.Id = id;
                    return authResponse.Token;
                });
           });
        }

        /// <summary>
        ///Build identity url for geting token 
        /// </summary>
        /// <returns></returns>
        private Uri BuildIdentityUrl()
        {
            return new Uri(Endpoint, "/v3/auth/tokens/");
        }

        /// <summary>
        ///Buidl key used to store token in tokenCach 
        /// </summary>
        private string BuildCacheTokenKey()
        {
            string user = User.Id != null ? User.Id :User.Domain + "/" + User.Name;
            string scope;
            if (Scope != null)
                scope = Scope.Project != null ? Scope.Project["id"] : (Scope.Domain != null ? Scope.Domain["id"] : null);
            else
                scope = "default_project";
            return String.Format("{0}:{1}", user ,scope);
        }

        /// <summary>
        ///Get authentication of the instance
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthScope> GetScope(CancellationToken cancellationToken)
        {
               var token = await GetTokenCache(cancellationToken);
               var domain = token.Domain != null ? new AuthScope((string)token.Domain["id"], AuthScopeType.Domain) : null;
               return token.Project != null ? new AuthScope((string)token.Project["id"], AuthScopeType.Project) : domain;
        }

        /// <summary>
        /// Get catalog from token cache
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HashSet<string>> GetRegion(CancellationToken cancellationToken)
        {
               var token =  await GetTokenCache(cancellationToken);
               var set = new HashSet<string>();
               foreach (var c in token.Catalog.Where<Catalog>(c => c.Type == ServiceType.Compute.Type))
               {
                   foreach (var endpoing in c.Endpoints)
                       set.Add(endpoing.Region);
               }
               return set;
        }
    }
}
