namespace OpenStack.Services.Networking.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using Rackspace.Threading;
    using Extension = Identity.V2.Extension;
    using ExtensionAlias = Identity.V2.ExtensionAlias;

    public static class NetworkingServiceExtensions
    {
        #region API Version

        public static Task<ReadOnlyCollectionPage<ApiVersion>> ListApiVersionsAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            return
                TaskBlocks.Using(
                    () => client.PrepareListApiVersionsAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2);
        }

        public static Task<ApiDetails> GetApiDetailsAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            return
                TaskBlocks.Using(
                    () => client.PrepareGetApiDetailsAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2);
        }

        #endregion

        #region Networks

        public static Task<Network> AddNetworkAsync(this INetworkingService client, NetworkData networkData, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (networkData == null)
                throw new ArgumentNullException("networkData");

            return
                TaskBlocks.Using(
                    () => client.PrepareAddNetworkAsync(new NetworkRequest(networkData), cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Network);
        }

        public static Task<ReadOnlyCollectionPage<Network>> ListNetworksAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            return
                TaskBlocks.Using(
                    () => client.PrepareListNetworksAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2);
        }

        public static Task<Network> GetNetworkAsync(this INetworkingService client, NetworkId networkId, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (networkId == null)
                throw new ArgumentNullException("networkId");

            return
                TaskBlocks.Using(
                    () => client.PrepareGetNetworkAsync(networkId, cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Network);
        }

        public static Task<Network> UpdateNetworkAsync(this INetworkingService client, NetworkId networkId, NetworkData networkData, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (networkId == null)
                throw new ArgumentNullException("networkId");
            if (networkData == null)
                throw new ArgumentNullException("networkData");

            return
                TaskBlocks.Using(
                    () => client.PrepareUpdateNetworkAsync(networkId, new NetworkRequest(networkData), cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Network);
        }

        public static Task RemoveNetworkAsync(this INetworkingService client, NetworkId networkId, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (networkId == null)
                throw new ArgumentNullException("networkId");

            return TaskBlocks.Using(
                () => client.PrepareRemoveNetworkAsync(networkId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Subnets

        public static Task<Subnet> AddSubnetAsync(this INetworkingService client, SubnetData subnetData, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (subnetData == null)
                throw new ArgumentNullException("subnetData");

            return
                TaskBlocks.Using(
                    () => client.PrepareAddSubnetAsync(new SubnetRequest(subnetData), cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Subnet);
        }

        public static Task<ReadOnlyCollectionPage<Subnet>> ListSubnetsAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            return
                TaskBlocks.Using(
                    () => client.PrepareListSubnetsAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2);
        }

        public static Task<Subnet> GetSubnetAsync(this INetworkingService client, SubnetId subnetId, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (subnetId == null)
                throw new ArgumentNullException("subnetId");

            return
                TaskBlocks.Using(
                    () => client.PrepareGetSubnetAsync(subnetId, cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Subnet);
        }

        public static Task<Subnet> UpdateSubnetAsync(this INetworkingService client, SubnetId subnetId, SubnetData subnetData, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (subnetId == null)
                throw new ArgumentNullException("subnetId");
            if (subnetData == null)
                throw new ArgumentNullException("subnetData");

            return
                TaskBlocks.Using(
                    () => client.PrepareUpdateSubnetAsync(subnetId, new SubnetRequest(subnetData), cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Subnet);
        }

        public static Task RemoveSubnetAsync(this INetworkingService client, SubnetId subnetId, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (subnetId == null)
                throw new ArgumentNullException("subnetId");

            return TaskBlocks.Using(
                () => client.PrepareRemoveSubnetAsync(subnetId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Ports

        public static Task<Port> AddPortAsync(this INetworkingService client, PortData portData, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (portData == null)
                throw new ArgumentNullException("portData");

            return
                TaskBlocks.Using(
                    () => client.PrepareAddPortAsync(new PortRequest(portData), cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Port);
        }

        public static Task<ReadOnlyCollectionPage<Port>> ListPortsAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            return
                TaskBlocks.Using(
                    () => client.PrepareListPortsAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2);
        }

        public static Task<Port> GetPortAsync(this INetworkingService client, PortId portId, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (portId == null)
                throw new ArgumentNullException("portId");

            return
                TaskBlocks.Using(
                    () => client.PrepareGetPortAsync(portId, cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Port);
        }

        public static Task<Port> UpdatePortAsync(this INetworkingService client, PortId portId, PortData portData, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (portId == null)
                throw new ArgumentNullException("portId");
            if (portData == null)
                throw new ArgumentNullException("portData");

            return
                TaskBlocks.Using(
                    () => client.PrepareUpdatePortAsync(portId, new PortRequest(portData), cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Port);
        }

        public static Task RemovePortAsync(this INetworkingService client, PortId portId, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (portId == null)
                throw new ArgumentNullException("portId");

            return TaskBlocks.Using(
                () => client.PrepareRemovePortAsync(portId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Extensions

        public static Task<ReadOnlyCollectionPage<Extension>> ListExtensionsAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            return
                TaskBlocks.Using(
                    () => client.PrepareListExtensionsAsync(cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2);
        }

        public static Task<Extension> GetExtensionAsync(this INetworkingService client, ExtensionAlias alias, CancellationToken cancellationToken)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            if (alias == null)
                throw new ArgumentNullException("alias");

            return
                TaskBlocks.Using(
                    () => client.PrepareGetExtensionAsync(alias, cancellationToken),
                    task => task.Result.SendAsync(cancellationToken))
                .Select(task => task.Result.Item2.Extension);
        }

        #endregion
    }
}
