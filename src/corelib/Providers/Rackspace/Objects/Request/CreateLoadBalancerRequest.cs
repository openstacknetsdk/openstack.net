using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateLoadBalancerRequest
    {
        [DataMember(Name = "loadBalancer")]
        public NewLoadBalancer LoadBalancer { get; set; }
    }

    [DataContract]
    internal class NewLoadBalancer
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        //[DataMember(Name = "nodes")]

        [DataMember(Name = "protocol")]
        public string Protocol { get; set; }

        [DataMember(Name = "virtualIps")]
        public IEnumerable<NewVirtualIP> VirtualIPs { get; set; }

        [DataMember(Name = "port")]
        public int Port { get; set; }

        [DataMember(Name = "halfClosed", EmitDefaultValue = false)]
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

    [DataContract]
    internal class NewVirtualIP
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
