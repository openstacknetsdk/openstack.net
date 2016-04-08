using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Authentication;
using OpenStack.Serialization;
using System.Threading;
using OpenStack.Images.v2.Serialization;
using Flurl.Http;
using Flurl.Extensions;

namespace OpenStack.Images.v2
{
    /// <summary>
    ///Ihe Openstack image service 
    ///<seealso href="http://developer.openstack.org/api-ref-image-v2.html"/>
    /// </summary>
    public class ImageService
    {

        internal readonly ImageApiBuilder _imageApiBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        public ImageService(IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
        {
            _imageApiBuilder = new ImageApiBuilder(ServiceType.Image, authenticationProvider, region, useInternalUrl);
        }

        /// <inheritdoc cref="ImageApiBuilder.ListImageAsync(CancellationToken)" />
        public async Task<IEnumerable<ImageOptions>> ListImageAsync(CancellationToken cancellationToken=default(CancellationToken))
        {
            return await _imageApiBuilder.ListImageAsync(cancellationToken);
        }
    }
}
