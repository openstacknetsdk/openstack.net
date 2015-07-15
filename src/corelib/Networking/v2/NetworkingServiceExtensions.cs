using System.Collections.Generic;
using System.Threading.Tasks;
using OpenStack.Networking.v2;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for an <see cref="INetworkingService"/> instance.
    /// </summary>
    public static class NetworkingServiceExtensions
    {
        public static IEnumerable<Network> ListNetworks(this INetworkingService networkingService)
        {
            return networkingService.ListNetworksAsync().ForceSynchronous();
        }

        public static Network GetNetwork(this INetworkingService networkingService, string networkId)
        {
            return networkingService.GetNetworkAsync(networkId).ForceSynchronous();
        }

        public static IEnumerable<Network> CreateNetworks(this INetworkingService networkingService, IEnumerable<NetworkDefinition> networks)
        {
            return networkingService.CreateNetworksAsync(networks).ForceSynchronous();
        }

        public static Network CreateNetwork(this INetworkingService networkingService, NetworkDefinition network)
        {
            return networkingService.CreateNetworkAsync(network).ForceSynchronous();
        }

        public static Network UpdateNetwork(this INetworkingService networkingService, string networkId, NetworkDefinition network)
        {
            return networkingService.UpdateNetworkAsync(networkId, network).ForceSynchronous();
        }

        public static void DeleteNetwork(this INetworkingService networkingService, string networkId)
        {
            networkingService.DeleteNetworkAsync(networkId).ForceSynchronous();
        }
    }
}