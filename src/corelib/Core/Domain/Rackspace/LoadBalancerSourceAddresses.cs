using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerSourceAddresses
    {
        [JsonProperty("ipv6Public")]
        public string PublicIPv6 { get; set; }

        [JsonProperty("ipv4Servicenet")]
        public string ServiceNetIPv4 { get; set; }

        [JsonProperty("ipv4Public")]
        public string PublicIPv4 { get; set; }
    }
}