namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class CloudNetwork
    {
        /// <summary>
        /// The network ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The CIDR for an isolated network.
        /// </summary>
        [JsonProperty("cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// The name of the network. ServiceNet is labeled as private and PublicNet 
        /// is labeled as public in the network list.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
