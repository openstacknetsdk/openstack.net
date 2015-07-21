using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// The default implementation of the OpenStack Networking Service.
    /// </summary>
    /// <preliminary/>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack Networking API v2 Reference</seealso>
    public class NetworkingService
    {
        private readonly NetworkingApiBuilder _networkingApiBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkingService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        public NetworkingService(IAuthenticationProvider authenticationProvider, string region)
        {
            _networkingApiBuilder = new NetworkingApiBuilder(ServiceType.Networking, authenticationProvider, region);
        }

        /// <inheritdoc cref="NetworkingApiBuilder.ListNetworksAsync" />
        public Task<IEnumerable<Network>> ListNetworksAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .ListNetworksAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<NetworkCollection>()
                .AsEnumerable<NetworkCollection, Network>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.GetNetworkAsync" />
        public Task<Network> GetNetworkAsync(string networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .GetNetworkAsync(networkId, cancellationToken)
                .SendAsync()
                .ReceiveJson<Network>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateNetworkAsync" />
        public Task<Network> CreateNetworkAsync(NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .CreateNetworkAsync(network, cancellationToken)
                .SendAsync()
                .ReceiveJson<Network>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateNetworksAsync" />
        public Task<IEnumerable<Network>> CreateNetworksAsync(IEnumerable<NetworkDefinition> networks, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .CreateNetworksAsync(networks, cancellationToken)
                .SendAsync()
                .ReceiveJson<NetworkCollection>()
                .AsEnumerable<NetworkCollection, Network>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdateNetworkAsync" />
        public Task<Network> UpdateNetworkAsync(string networkId, NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .UpdateNetworkAsync(networkId, network, cancellationToken)
                .SendAsync()
                .ReceiveJson<Network>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeleteNetworkAsync" />
        public Task DeleteNetworkAsync(string networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .DeleteNetworkAsync(networkId, cancellationToken)
                .SendAsync();
        }
    }
}
