using Newtonsoft.Json;

namespace net.openstack.Providers.Rackspace.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class NewLoadBalancerVirtualIP
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}