namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterfaceAddress
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("network_id")]
        public string NetworkId { get; set; }

        [JsonProperty("network_label")]
        public string NetworkLabel { get; set; }
    }
}