namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterfaceAddress
    {
        [JsonProperty("address")]
        public string Address { get; private set; }

        [JsonProperty("network_id")]
        public string NetworkId { get; private set; }

        [JsonProperty("network_label")]
        public string NetworkLabel { get; private set; }
    }
}