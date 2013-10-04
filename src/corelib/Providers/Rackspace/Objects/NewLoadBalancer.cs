using System.Collections.Generic;
using Newtonsoft.Json;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Providers.Rackspace.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class NewLoadBalancer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nodes")]
        public IEnumerable<LoadBalancerNode> Nodes { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("virtualIps")]
        public IEnumerable<NewLoadBalancerVirtualIP> VirtualIPs { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("halfClosed", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HalfClosed { get; set; }

        //[DataMember(Name = "accessList", EmitDefaultValue = false)]

        //[DataMember(Name = "algorithm", EmitDefaultValue = false)]
        //public string Algorithm { get; set; }

        //[DataMember(Name = "connectionLogging", EmitDefaultValue = false)]

        //[DataMember(Name = "connectionThrottle", EmitDefaultValue = false)]

        //[DataMember(Name = "healthMonitor", EmitDefaultValue = false)]

        //[DataMember(Name = "metadata", EmitDefaultValue = false)]

        //[DataMember(Name = "timeout", EmitDefaultValue = false)]
        //public int Timeout { get; set; }

        //[DataMember(Name = "sessionPersistence", EmitDefaultValue = false)]        
    }
}