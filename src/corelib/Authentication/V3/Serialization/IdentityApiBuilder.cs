using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Serialization;
using System.Threading;
using Flurl.Http;
using Flurl;
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
        protected readonly IAuthenticationProvider AuthProvider;

        /// <summary>
        /// 
        /// </summary>
        protected readonly ServiceEndpoint Endpoint; 


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="provider"></param>
        /// <param name="region"></param>
        /// <param name="useInternal"></param>
        public IdentityApiBuilder(IServiceType serviceType, IAuthenticationProvider provider, string region, bool useInternal)
        {
            AuthProvider = provider;
            Endpoint = new ServiceEndpoint(serviceType, provider, region, useInternal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PreparedRequest> ListProjectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
            return endpoint
                .AppendPathSegment("projects")
                .Authenticate(AuthProvider)
                .PrepareGet();
        }
    }
}
