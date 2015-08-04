using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;
using OpenStack.Networking.v2.Serialization;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// The default implementation of the OpenStack Networking Service.
    /// </summary>
    /// <seealso href="https://wiki.openstack.org/wiki/Neutron/APIv2-specification">OpenStack Networking API v2 Overview</seealso>
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

        #region Networks
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
        public Task<Network> GetNetworkAsync(Identifier networkId, CancellationToken cancellationToken = default(CancellationToken))
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
        public Task<Network> UpdateNetworkAsync(Identifier networkId, NetworkDefinition network, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .UpdateNetworkAsync(networkId, network, cancellationToken)
                .SendAsync()
                .ReceiveJson<Network>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeleteNetworkAsync" />
        public Task DeleteNetworkAsync(Identifier networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .DeleteNetworkAsync(networkId, cancellationToken)
                .SendAsync();
        }
        #endregion

        #region Subnets

        /// <inheritdoc cref="NetworkingApiBuilder.ListSubnetsAsync" />
        public Task<IEnumerable<Subnet>> ListSubnetsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .ListSubnetsAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<SubnetCollection>()
                .AsEnumerable<SubnetCollection, Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateSubnetAsync" />
        public Task<Subnet> CreateSubnetAsync(SubnetCreateDefinition subnet, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .CreateSubnetAsync(subnet, cancellationToken)
                .SendAsync()
                .ReceiveJson<Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateSubnetsAsync" />
        public Task<IEnumerable<Subnet>> CreateSubnetsAsync(IEnumerable<SubnetCreateDefinition> subnets, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .CreateSubnetsAsync(subnets, cancellationToken)
                .SendAsync()
                .ReceiveJson<SubnetCollection>()
                .AsEnumerable<SubnetCollection, Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.GetSubnetAsync" />
        public Task<Subnet> GetSubnetAsync(Identifier subnetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .GetSubnetAsync(subnetId, cancellationToken)
                .SendAsync()
                .ReceiveJson<Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdateSubnetAsync" />
        public Task<Subnet> UpdateSubnetAsync(Identifier subnetId, SubnetUpdateDefinition subnet, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .UpdateSubnetAsync(subnetId, subnet, cancellationToken)
                .SendAsync()
                .ReceiveJson<Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeleteSubnetAsync" />
        public Task DeleteSubnetAsync(Identifier subnetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .DeleteSubnetAsync(subnetId, cancellationToken)
                .SendAsync();
        }
        #endregion

        #region Ports

        /// <inheritdoc cref="NetworkingApiBuilder.ListPortsAsync" />
        public Task<IEnumerable<Port>> ListPortsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .ListPortsAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<PortCollection>()
                .AsEnumerable<PortCollection, Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreatePortAsync" />
        public Task<Port> CreatePortAsync(PortCreateDefinition port, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .CreatePortAsync(port, cancellationToken)
                .SendAsync()
                .ReceiveJson<Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreatePortsAsync" />
        public Task<IEnumerable<Port>> CreatePortsAsync(IEnumerable<PortCreateDefinition> ports, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .CreatePortsAsync(ports, cancellationToken)
                .SendAsync()
                .ReceiveJson<PortCollection>()
                .AsEnumerable<PortCollection, Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.GetPortAsync" />
        public Task<Port> GetPortAsync(Identifier portId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .GetPortAsync(portId, cancellationToken)
                .SendAsync()
                .ReceiveJson<Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdatePortAsync" />
        public Task<Port> UpdatePortAsync(Identifier portId, PortUpdateDefinition port, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .UpdatePortAsync(portId, port, cancellationToken)
                .SendAsync()
                .ReceiveJson<Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeletePortAsync" />
        public Task DeletePortAsync(Identifier portId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _networkingApiBuilder
                .DeletePortAsync(portId, cancellationToken)
                .SendAsync();
        }
        #endregion
    }
}
