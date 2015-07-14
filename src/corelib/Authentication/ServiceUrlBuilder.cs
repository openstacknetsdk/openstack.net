using System.Threading;
using System.Threading.Tasks;
using Flurl;

namespace OpenStack.Authentication
{
    internal class ServiceUrlBuilder
    {
        private readonly ServiceType _serviceType;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly string _region;
        private readonly bool _useInternalUrl;

        public ServiceUrlBuilder(ServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl)
        {
            _serviceType = serviceType;
            _authenticationProvider = authenticationProvider;
            _region = region;
            _useInternalUrl = useInternalUrl;
        }

        public async Task<Url> GetEndpoint(CancellationToken cancellationToken)
        {
            string endpoint = await _authenticationProvider.GetEndpoint(_serviceType, _region, _useInternalUrl, cancellationToken).ConfigureAwait(false);
            return new Url(endpoint);
        }
    }
}