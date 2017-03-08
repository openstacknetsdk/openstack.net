using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;
using OpenStack.Networking.v2.Serialization;
using OpenStack.ObjectStorage.v1.Serialization;

namespace OpenStack.ObjectStorage.v1
{
    /// <summary>
    /// The OpenStack Networking Service.
    /// </summary>
    /// <seealso href="https://wiki.openstack.org/wiki/Neutron/APIv2-specification">OpenStack Networking API v2 Overview</seealso>
    /// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack Networking API v2 Reference</seealso>
    public class ObjectStorageService
    {
        internal readonly ObjectStorageApiBuilder _objectStorageApiBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectStorageService"/> class.
        /// </summary>
        /// <param name="authenticationProvider">The authentication provider.</param>
        /// <param name="region">The region.</param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
        public ObjectStorageService(IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
        {
            _objectStorageApiBuilder = new ObjectStorageApiBuilder(ServiceType.ObjectStorage, authenticationProvider, region, useInternalUrl);
        }

        #region Containers
        /// <inheritdoc cref="ObjectStorageApiBuilder.ListContainersAsync(CancellationToken)" />
        public async Task<IEnumerable<Container>> ListContainersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _objectStorageApiBuilder
                .ListContainersAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<ContainerCollection>();
        }

        /// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerContentAsync(string, CancellationToken)" />
        public Task<ContainerObjectCollection> GetContainerContentAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .GetContainerContentAsync(containerId, cancellationToken)
                .SendAsync()
                .ReceiveJson<ContainerObjectCollection>();
        }

        /// <inheritdoc cref="ObjectStorageApiBuilder.CreateContainerAsync(string, CancellationToken)" />
        public Task<Container> CreateContainerAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .CreateContainerAsync(containerId, cancellationToken)
                .SendAsync()
                .ReceiveJson<Container>();
        }

        /// <inheritdoc cref="ObjectStorageApiBuilder.SaveContainerMetadataAsync(string, ContainerMetadataCollection, CancellationToken)" />
        public async Task<ContainerMetadataCollection> SaveContainerMetadataAsync(string containerId, ContainerMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _objectStorageApiBuilder
                .SaveContainerMetadataAsync(containerId, metadataCollection, cancellationToken)
                .SendAsync()
                .ReceiveString();
        }

        /// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerMetadataAsync(string, CancellationToken)" />
        public Task<ContainerMetadataCollection> ReadContainerMetadataAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .ReadContainerMetadataAsync(containerId, cancellationToken)
                .SendAsync()
                .ReceiveString();
        }

        /// <inheritdoc cref="ObjectStorageApiBuilder.DeleteContainerAsync(string, CancellationToken)" />
        public Task DeleteContainerAsync(Identifier networkId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .DeleteContainerAsync(networkId, cancellationToken)
                .SendAsync();
        }
        #endregion

        #region Subnets

        /// <inheritdoc cref="NetworkingApiBuilder.ListSubnetsAsync" />
        public async Task<IEnumerable<Subnet>> ListSubnetsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _objectStorageApiBuilder
                .ListSubnetsAsync(cancellationToken)
                .SendAsync()
                .ReceiveJson<SubnetCollection>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateSubnetAsync" />
        public Task<Subnet> CreateSubnetAsync(SubnetCreateDefinition subnet, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .CreateSubnetAsync(subnet, cancellationToken)
                .SendAsync()
                .ReceiveJson<Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreateSubnetsAsync" />
        public async Task<IEnumerable<Subnet>> CreateSubnetsAsync(IEnumerable<SubnetCreateDefinition> subnets, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _objectStorageApiBuilder
                .CreateSubnetsAsync(subnets, cancellationToken)
                .SendAsync()
                .ReceiveJson<SubnetCollection>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.GetSubnetAsync" />
        public Task<Subnet> GetSubnetAsync(Identifier subnetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .GetSubnetAsync(subnetId, cancellationToken)
                .SendAsync()
                .ReceiveJson<Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdateSubnetAsync" />
        public Task<Subnet> UpdateSubnetAsync(Identifier subnetId, SubnetUpdateDefinition subnet, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .UpdateSubnetAsync(subnetId, subnet, cancellationToken)
                .SendAsync()
                .ReceiveJson<Subnet>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeleteSubnetAsync" />
        public Task DeleteSubnetAsync(Identifier subnetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .DeleteSubnetAsync(subnetId, cancellationToken)
                .SendAsync();
        }
        #endregion

        #region Ports

        /// <inheritdoc cref="NetworkingApiBuilder.ListPortsAsync{T}" />
        public async Task<IEnumerable<Port>> ListPortsAsync(PortListOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _objectStorageApiBuilder.ListPortsAsync<PortCollection>(options, cancellationToken);
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreatePortAsync" />
        public Task<Port> CreatePortAsync(PortCreateDefinition port, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .CreatePortAsync(port, cancellationToken)
                .SendAsync()
                .ReceiveJson<Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.CreatePortsAsync" />
        public async Task<IEnumerable<Port>> CreatePortsAsync(IEnumerable<PortCreateDefinition> ports, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _objectStorageApiBuilder
                .CreatePortsAsync(ports, cancellationToken)
                .SendAsync()
                .ReceiveJson<PortCollection>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.GetPortAsync" />
        public Task<Port> GetPortAsync(Identifier portId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .GetPortAsync(portId, cancellationToken)
                .SendAsync()
                .ReceiveJson<Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.UpdatePortAsync" />
        public Task<Port> UpdatePortAsync(Identifier portId, PortUpdateDefinition port, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .UpdatePortAsync(portId, port, cancellationToken)
                .SendAsync()
                .ReceiveJson<Port>();
        }

        /// <inheritdoc cref="NetworkingApiBuilder.DeletePortAsync" />
        public Task DeletePortAsync(Identifier portId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _objectStorageApiBuilder
                .DeletePortAsync(portId, cancellationToken)
                .SendAsync();
        }
        #endregion
    }
}
