using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2
{
    /// <summary>
    /// Represents the definition of a network resource of the <see cref="INetworkingService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "network")]
    public class NetworkDefinition
    {
        /// <summary>
        /// The network name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether this network is externally accessible.
        /// </summary>
        [JsonProperty("router:external")]
        public bool? IsExternal { get; set; }

        /// <summary>
        /// The physical network where this network object is implemented. 
        /// <para>
        /// The Networking API v2.0 does not provide a way to list available physical networks.
        /// </para>
        /// <para>
        /// For example, the Open vSwitch plug-in configuration file defines a symbolic name that maps to specific bridges on each Compute host.
        /// </para>
        /// </summary>
        [JsonProperty("provider:physical_network")]
        public string PhysicalNetwork { get; set; }

        /// <summary>
        /// The type of physical network that maps to this network resource. Examples are flat, vlan, vxlan, and gre. 
        /// </summary>
        [JsonProperty("provider:network_type")]
        public string NetworkType { get; set; }

        /// <summary>
        /// An isolated segment on the physical network.
        /// <para>
        /// The network_type attribute defines the segmentation model.
        /// </para>
        /// <para>
        /// For example, if the network_type value is vlan, this ID is a vlan identifier. If the network_type value is gre, this ID is a gre key.
        /// </para>
        /// </summary>
        [JsonProperty("provider:segmentation_id")]
        public string SegmentationId { get; set; }
    }
}