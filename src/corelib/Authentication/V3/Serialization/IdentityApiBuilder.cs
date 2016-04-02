using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Serialization;
using System.Threading;
using Flurl.Http;
using Flurl;
using OpenStack.Authentication.V3.Auth;
using Flurl.Extensions;

namespace OpenStack.Authentication.V3.Serialization
{
    /// <summary>
    /// 
    /// </summary>
   public class IdentityApiBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IIAuthenticationProvider _authProvider;

        /// <summary>
        /// 
        /// </summary>
        protected readonly ServiceEndpoint _endpoint; 


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="provider"></param>
        /// <param name="region"></param>
        /// <param name="useInternal"></param>
        public IdentityApiBuilder(IServiceType serviceType, IIAuthenticationProvider provider, string region, bool useInternal)
        {
            _authProvider = provider;
            _endpoint = new ServiceEndpoint(serviceType, provider, region, useInternal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Url> PrepareUserUrl(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url url = await _endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
            var userId = await _authProvider.GetUserId(cancellationToken);
            url.AppendPathSegment($"users/{userId}");
            return url;
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PreparedRequest> ListProjectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await PrepareUserUrl(cancellationToken);
            return endpoint
                .AppendPathSegment($"projects")
                .Authenticate(_authProvider)
                .PrepareGet();
        }
    }
}
