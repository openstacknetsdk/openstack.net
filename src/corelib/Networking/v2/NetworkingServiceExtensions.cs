using System.Collections.Generic;
using OpenStack.Networking.v2;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for an <see cref="NetworkingService"/> instance.
    /// </summary>
    public static class NetworkingServiceExtensions
    {
        #region Networks
        /// <summary>
        /// Lists all networks associated with the account.
        /// </summary>
        /// <returns>
        /// A collection of network resources associated with the account.
        /// </returns>
        public static IEnumerable<Network> ListNetworks(this NetworkingService networkingService)
        {
            return networkingService.ListNetworksAsync().ForceSynchronous();
        }

        /// <summary>
        /// Gets the specified network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        /// <returns>
        /// The network associated with the specified identifier.
        /// </returns>
        public static Network GetNetwork(this NetworkingService networkingService, string networkId)
        {
            return networkingService.GetNetworkAsync(networkId).ForceSynchronous();
        }

        /// <summary>
        /// Bulk creates multiple networks.
        /// </summary>
        /// <param name="networks">The network definitions.</param>
        /// <returns>
        /// The created networks.
        /// </returns>
        public static IEnumerable<Network> CreateNetworks(this NetworkingService networkingService, IEnumerable<NetworkDefinition> networks)
        {
            return networkingService.CreateNetworksAsync(networks).ForceSynchronous();
        }

        /// <summary>
        /// Creates a network.
        /// </summary>
        /// <param name="network">The network definition.</param>
        /// <returns>
        /// The created network.
        /// </returns>
        public static Network CreateNetwork(this NetworkingService networkingService, NetworkDefinition network)
        {
            return networkingService.CreateNetworkAsync(network).ForceSynchronous();
        }

        /// <summary>
        /// Updates the specified network.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="network">The updated network definition.</param>
        /// <returns>
        /// The updated network.
        /// </returns>
        public static Network UpdateNetwork(this NetworkingService networkingService, string networkId, NetworkDefinition network)
        {
            return networkingService.UpdateNetworkAsync(networkId, network).ForceSynchronous();
        }

        /// <summary>
        /// Deletes the specified network.
        /// </summary>
        /// <param name="networkId">The network identifier.</param>
        public static void DeleteNetwork(this NetworkingService networkingService, string networkId)
        {
            networkingService.DeleteNetworkAsync(networkId).ForceSynchronous();
        }
        #endregion

        #region Subnets
        /// <summary>
        /// Lists all subnets associated with the account.
        /// </summary>
        /// <returns>
        /// A collection of subnet resources associated with the account.
        /// </returns>
        public static IEnumerable<Subnet> ListSubnets(this NetworkingService networkingService)
        {
            return networkingService.ListSubnetsAsync().ForceSynchronous();
        }

        /// <summary>
        /// Creates a subnet.
        /// </summary>
        /// <param name="subnet">The subnet definition.</param>
        /// <returns>
        /// The created subnet.
        /// </returns>
        public static Subnet CreateSubnet(this NetworkingService networkingService, SubnetCreateDefinition subnet)
        {
            return networkingService.CreateSubnetAsync(subnet).ForceSynchronous();
        }

        /// <summary>
        /// Bulk creates multiple subnets.
        /// </summary>
        /// <param name="subnets">The subnet definitions.</param>
        /// <returns>
        /// The created subnets.
        /// </returns>
        public static IEnumerable<Subnet> CreateSubnets(this NetworkingService networkingService, IEnumerable<SubnetCreateDefinition> subnets)
        {
            return networkingService.CreateSubnetsAsync(subnets).ForceSynchronous();
        }

        /// <summary>
        /// Gets the specified subnet.
        /// </summary>
        /// <param name="subnetId">The subnet identifier.</param>
        /// <returns>
        /// The subnet associated with the specified identifier.
        /// </returns>
        public static Subnet GetSubnet(this NetworkingService networkingService, string subnetId)
        {
            return networkingService.GetSubnetAsync(subnetId).ForceSynchronous();
        }

        /// <summary>
        /// Updates the specified subnet.
        /// </summary>
        /// <param name="subnetId"></param>
        /// <param name="subnet">The updated subnet definition.</param>
        /// <returns>
        /// The updated port.
        /// </returns>
        public static Subnet UpdateSubnet(this NetworkingService networkingService, string subnetId, SubnetUpdateDefinition subnet)
        {
            return networkingService.UpdateSubnetAsync(subnetId, subnet).ForceSynchronous();
        }

        /// <summary>
        /// Deletes the specified subnet.
        /// </summary>
        /// <param name="subnetId">The subnet identifier.</param>
        public static void DeleteSubnet(this NetworkingService networkingService, string subnetId)
        {
            networkingService.DeleteSubnetAsync(subnetId).ForceSynchronous();
        }
        #endregion

        #region Ports
        /// <summary>
        /// Lists all ports associated with the account.
        /// </summary>
        /// <returns>
        /// A collection of port resources associated with the account.
        /// </returns>
        public static IEnumerable<Port> ListPorts(this NetworkingService networkingService)
        {
            return networkingService.ListPortsAsync().ForceSynchronous();
        }

        /// <summary>
        /// Creates a port.
        /// </summary>
        /// <param name="port">The port definition.</param>
        /// <returns>
        /// The created port.
        /// </returns>
        public static Port CreatePort(this NetworkingService networkingService, PortCreateDefinition port)
        {
            return networkingService.CreatePortAsync(port).ForceSynchronous();
        }

        /// <summary>
        /// Bulk creates multiple ports.
        /// </summary>
        /// <param name="ports">The port definitions.</param>
        /// <returns>
        /// The created ports.
        /// </returns>
        public static IEnumerable<Port> CreatePorts(this NetworkingService networkingService, IEnumerable<PortCreateDefinition> ports)
        {
            return networkingService.CreatePortsAsync(ports).ForceSynchronous();
        }

        /// <summary>
        /// Gets the specified port.
        /// </summary>
        /// <param name="portId">The port identifier.</param>
        /// <returns>
        /// The port associated with the specified identifier.
        /// </returns>
        public static Port GetPort(this NetworkingService networkingService, Identifier portId)
        {
            return networkingService.GetPortAsync(portId).ForceSynchronous();
        }

        /// <summary>
        /// Updates the specified port.
        /// </summary>
        /// <param name="portId"></param>
        /// <param name="port">The updated port definition.</param>
        /// <returns>
        /// The updated port.
        /// </returns>
        public static Port UpdatePort(this NetworkingService networkingService, Identifier portId, PortUpdateDefinition port)
        {
            return networkingService.UpdatePortAsync(portId, port).ForceSynchronous();
        }

        /// <summary>
        /// Deletes the specified port.
        /// </summary>
        /// <param name="portId">The port identifier.</param>
        public static void DeletePort(this NetworkingService networkingService, Identifier portId)
        {
            networkingService.DeletePortAsync(portId).ForceSynchronous();
        }
        #endregion
    }
}