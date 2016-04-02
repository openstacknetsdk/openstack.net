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
    /// 
    /// </summary>
    public class OpenstackAuthenticationProvider : IAuthenticationProvider
    {
        private readonly UserCredential UserCredential;
        private readonly Uri Endpoint;
        private TokenCache TokenCache;

        /// <summary>
        /// 
        /// </summary>
        public readonly AuthUser User;

        /// <summary>
        /// 
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
            UserCredential = new UserCredential(User, Scope);
            TokenCache = new TokenCache();
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
            var token = GetToken(BuildIdentityUrl(), UserCredential, cancellationToken).Catalog;
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

        private Token GetToken(Uri uri, UserCredential userCredential, CancellationToken cancellationToken)
        {
            var key = BuildCacheTokenKey();
            IDictionary<string, UserCredential> authRequest = new Dictionary<string, UserCredential>
            {
                {
                    "auth", userCredential
                }
            };
            var t = TokenCache.Get(key, () =>
            {
                var lazyclient = new LazyHttpClient(uri);
                var resp = lazyclient.LazyJsonPost(authRequest);
                var token = resp.ConvertJsonContentToObject<AuthResponse>().Token;
                token.SetId(resp.GetHeaderValue("X-Subject-Token"));
                return token;
            }, false);
            return t;
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
               return GetToken(BuildIdentityUrl(), UserCredential, cancellationToken).Id;
           });
         }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uri BuildIdentityUrl()
        {
            var scheme = Endpoint.Scheme;
            var host = Endpoint.Host;
            var port = Endpoint.Port;
            return new Uri($"{scheme}://{host}:{port}/v3/auth/tokens");
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AuthScope> GetScope(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew( () =>
           {
               var token = GetToken(BuildIdentityUrl(), UserCredential, cancellationToken);
               var domain = token.Domain != null ? new AuthScope((string)token.Domain["id"], AuthScopeType.Domain) : null;
               return token.Project != null ? new AuthScope((string)token.Project["id"], AuthScopeType.Project) : domain;
           });
        }
    }
}
