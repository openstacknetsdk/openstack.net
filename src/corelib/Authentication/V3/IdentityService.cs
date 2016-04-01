using OpenStack.Authentication.V3.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityService  
    {
        private IdentityApi _identityApi; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityEndpoint"></param>
        /// <param name="credential"></param>
        public IdentityService(Uri identityEndpoint, Credential credential)
        {
            this._identityApi = new IdentityApi(identityEndpoint, credential);
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
            return this._identityApi.GetEndpoint(serviceType, region, useInternalUrl, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetToken(CancellationToken cancellationToken)
        {
            return this._identityApi.GetToken(cancellationToken);
        }
    }
}
