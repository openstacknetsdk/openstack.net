namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterface
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("ip_addresses")]
        public VirtualInterfaceAddress[] Addresses { get; private set; }

        [JsonProperty("mac_address")]
        public string MACAddress { get; private set; }
    }
}
