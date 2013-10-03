using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerConnectionLogging
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}