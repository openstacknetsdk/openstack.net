using System.Threading;
using System.Threading.Tasks;
using Flurl;

namespace OpenStack.Authentication
{
    /// <summary>
    /// Creates urls
    /// </summary>
    public class ServiceUrlBuilder
    {
        private readonly IServiceType _serviceType;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly string _region;
        private readonly bool _useInternalUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUrlBuilder"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="useInternalUrl">Specifies if internal URLs should be used.</param>
        public ServiceUrlBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
        {
            _serviceType = serviceType;
            _authenticationProvider = authenticationProvider;
            _region = region;
            _useInternalUrl = useInternalUrl;
        }

        /// <summary>
        /// Gets the service endpoint.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Url"/> representing the service endpoint.</returns>
        public async Task<Url> GetEndpoint(CancellationToken cancellationToken)
        {
            string endpoint = await _authenticationProvider.GetEndpoint(_serviceType, _region, _useInternalUrl, cancellationToken).ConfigureAwait(false);
            return new Url(endpoint);
        }
    }
}