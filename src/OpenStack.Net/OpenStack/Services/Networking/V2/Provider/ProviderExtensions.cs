namespace OpenStack.Services.Networking.V2.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;
    using Rackspace.Threading;
    using ExtensionAlias = Identity.V2.ExtensionAlias;

    public static class ProviderExtensions
    {
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("provider");

        public static Task<bool> SupportsNetworkProviderAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            return client.ListExtensionsAsync(cancellationToken)
                .Select(task => task.Result.Any(i => ExtensionAlias.Equals(i.Alias)));
        }

        public static string GetProviderPhysicalNetwork(this NetworkData network)
        {
            if (network == null)
                throw new ArgumentNullException("network");

            JToken value;
            if (!network.ExtensionData.TryGetValue("provider:physical_network", out value))
                return null;

            return value.ToObject<string>();
        }

        public static NetworkType GetProviderNetworkType(this NetworkData network)
        {
            if (network == null)
                throw new ArgumentNullException("network");

            JToken value;
            if (!network.ExtensionData.TryGetValue("provider:network_type", out value))
                return null;

            return value.ToObject<NetworkType>();
        }

        public static SegmentationId GetProviderSegmentationId(this NetworkData network)
        {
            if (network == null)
                throw new ArgumentNullException("network");

            JToken value;
            if (!network.ExtensionData.TryGetValue("provider:segmentation_id", out value))
                return null;

            return value.ToObject<SegmentationId>();
        }

        public static NetworkData WithProviderPhysicalNetwork(this NetworkData network, string physicalNetwork)
        {
            var extensionData = network.ExtensionData.SetItem("provider:physical_network", JToken.FromObject(physicalNetwork));
            return network.WithExtensionData(extensionData);
        }

        public static NetworkData WithProviderNetworkType(this NetworkData network, NetworkType networkType)
        {
            var extensionData = network.ExtensionData.SetItem("provider:network_type", JToken.FromObject(networkType));
            return network.WithExtensionData(extensionData);
        }

        public static NetworkData WithProviderSegmentationId(this NetworkData network, SegmentationId segmentationId)
        {
            var extensionData = network.ExtensionData.SetItem("provider:segmentation_id", JToken.FromObject(segmentationId));
            return network.WithExtensionData(extensionData);
        }
    }
}
