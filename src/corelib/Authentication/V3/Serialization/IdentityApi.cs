using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Serialization;
using System.Threading;

namespace OpenStack.Authentication.V3.Serialization
{
    /// <summary>
    /// 
    /// </summary>
   public class IdentityApi
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
        /// <param name="provider"></param>
        /// <param name="endpoint"></param>
        public IdentityApi(IAuthenticationProvider provider, ServiceEndpoint endpoint)
        {
            AuthProvider = provider;
            Endpoint = endpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <param name="queryString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TPage> ListProjectSummaryAsync<TPage>(IQueryStringBuilder queryString, CancellationToken cancellationToken = default(CancellationToken))
            where TPage : IPageBuilder<TPage>, IEnumerable<IServiceResource>
        {
            Url initialRequestUrl = await BuildListServersUrl(queryString, cancellationToken).ConfigureAwait(false);
            return await Endpoint.GetResourcePageAsync<TPage>(initialRequestUrl, cancellationToken)
                .PropogateOwnerToChildren(this).ConfigureAwait(false);
        }

    }
}
