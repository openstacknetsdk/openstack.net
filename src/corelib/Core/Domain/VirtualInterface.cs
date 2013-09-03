namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterface
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ip_addresses")]
        public VirtualInterfaceAddress[] Addresses { get; set; }

        [JsonProperty("mac_address")]
        public string MACAddress { get; set; }
    }
}
