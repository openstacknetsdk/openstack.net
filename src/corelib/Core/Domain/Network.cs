namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Network
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ip")]
        public AddressDetails[] Addresses { get; set; }
    }
}