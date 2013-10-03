using System.Collections.Generic;
using Newtonsoft.Json;
using net.openstack.Core.Providers.Rackspace;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleLoadBalancer : ProviderStateBase<ICloudLoadBalancerProvider>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("protocol")]
        public LoadBalancerProtocol Protocol { get; set; }

        [JsonProperty("algorithm")]
        public LoadBalancerAlgorithm Algorithm { get; set; }

        [JsonProperty("status")]
        public LoadBalancerState Status { get; set; }

        [JsonProperty("nodeCount")]
        public int NodeCount { get; set; }

        [JsonProperty("virtualIps")]
        public IEnumerable<VirtualIP> VirtualIPs { get; set; }

        //[JsonProperty("created")]
        //public DateTimeOffset CreatedDate { get; set; }

        //[JsonProperty("updated")]
        //public DateTime UpdatedDate { get; set; }
    }
}