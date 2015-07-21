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
        /// Creates a network.
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
        /// Bulk creates multiple networks.
        /// </summary>
        /// <param name="network">The network definition.</param>
        /// <returns>
        /// The created networks.
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
    }
}