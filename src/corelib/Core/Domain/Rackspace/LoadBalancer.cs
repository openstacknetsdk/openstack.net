using System.Collections.Generic;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancer : SimpleLoadBalancer
    {
        [JsonProperty("nodes")]
        public IEnumerable<LoadBalancerNode> Nodes { get; set; }

        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("connectionLogging")]
        public LoadBalancerConnectionLogging ConnectionLogging { get; set; }

        [JsonProperty("sessionPersistance")]
        public LoadBalancerSessionPersistance SessionPersistance { get; set; }

        [JsonProperty("connectionThrottle")]
        public LoadBalancerConnectionThrottle ConnectionThrottle { get; set; }

        [JsonProperty("cluster")]
        public LoadBalancerCluster Cluster { get; set; }

        [JsonProperty("sourceAddress")]
        public LoadBalancerSourceAddresses SourceAddresses { get; set; }
    }
}
