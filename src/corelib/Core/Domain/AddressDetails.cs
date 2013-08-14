namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class AddressDetails
    {
        [JsonProperty("addr")]
        public string Address { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}