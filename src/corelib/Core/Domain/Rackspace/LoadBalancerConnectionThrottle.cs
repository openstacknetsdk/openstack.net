using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerConnectionThrottle
    {
        [JsonProperty("minConnections")]
        public int MinimalConnections { get; set; }

        [JsonProperty("maxConnections")]
        public int MaximumConnections { get; set; }

        [JsonProperty("maxConnectionRate")]
        public int MaximumConnectionRate { get; set; }

        [JsonProperty("rateInterval")]
        public int RateInterval { get; set; }
    }
}