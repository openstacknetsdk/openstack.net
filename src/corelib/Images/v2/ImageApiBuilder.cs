using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Serialization;
using OpenStack.Authentication;
using System.Threading;
using Flurl.Http;
using Flurl.Extensions;
using OpenStack.Images.v2.Serialization;

namespace OpenStack.Images.v2
{
    /// <summary>
    /// Builds requests to the Image API which can be further customized and then executed.
    /// <para>Intended for custom implementations.</para>
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-image-v2.html">OpenStack Image API v2 Reference</seealso>
    public class ImageApiBuilder
    {
        private readonly ServiceEndpoint _endpoint;
        private readonly IAuthenticationProvider _authProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageApiBuilder"/> class.
        /// </summary>
        /// <param name="serviceType">The service type for the desired image provider.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        public ImageApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
        {
            _authProvider = authenticationProvider;
            _endpoint = new ServiceEndpoint(serviceType, authenticationProvider, region, useInternalUrl);
        }

        #region Image
        /// <summary>
        /// List all images associated with the account
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// return the collections of resource associated with the accout
        /// </returns>
        public virtual async Task<PreparedRequest> BuildListImageAsync (CancellationToken cancellationToken)
        {
            var endpoint = await _endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
            return endpoint
                .AppendPathSegment("images")
                .Authenticate(_authProvider)
                .PrepareGet();
        }

        /// <inheritdoc cref="ImageApiBuilder.ListImageAsync(CancellationToken)" />
        public async Task<IEnumerable<ImageOptions>> ListImageAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await BuildListImageAsync(cancellationToken)
                 .SendAsync()
                 .ReceiveJson<ImageOptionsCollection>();
        }
        #endregion Image
    }
}
