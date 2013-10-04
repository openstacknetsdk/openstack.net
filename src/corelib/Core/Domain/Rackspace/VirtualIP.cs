using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualIP
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string IPAddress { get; set; }

        [JsonProperty("type")]
        public VirtualIPType Type { get; set; }

        [JsonProperty("ipVersion")]
        public string IPVersion { get; set; }
    }
}