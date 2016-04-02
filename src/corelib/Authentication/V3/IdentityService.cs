using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Authentication.V3.Serialization;
using System.Threading;
using OpenStack.Serialization;
using Flurl.Http;
using Flurl.Extensions;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityService
    {
        private readonly IdentityApiBuilder _identityApiBuilder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationProvider"></param>
        /// <param name="region"></param>
        /// <param name="useInternalUrl"></param>
        public IdentityService(IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
        {
            _identityApiBuilder = new IdentityApiBuilder(ServiceType.Identtity, authenticationProvider, region, useInternalUrl);
        }

        #region Project
        /// <inheritdoc cref="IdentityApiBuilder.ListProjectAsync(System.Threading.CancellationToken)"></inheritdoc>
        public async Task<IEnumerable<Project>> ListNetworksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _identityApiBuilder
                .ListProjectAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<ProjectColletion>();
        }
        #endregion Project

    }
}