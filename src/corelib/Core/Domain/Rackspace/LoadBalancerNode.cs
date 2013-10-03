using Newtonsoft.Json;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerNode
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string IPAddress { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("condition")]
        public LoadBalancerNodeCondition Condition { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("weight", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Weight { get; set; }

        [JsonProperty("type")]
        public LoadBalancerNodeType Type { get; set; }
    }
}
