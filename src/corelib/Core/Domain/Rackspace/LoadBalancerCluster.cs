using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerCluster
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}