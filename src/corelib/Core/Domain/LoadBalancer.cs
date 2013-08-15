using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Providers.Rackspace;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class SimpleLoadBalancer : ProviderStateBase<ICloudLoadBalancerProvider>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Protocol { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string Algorithm { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int NodeCount { get; set; }

        [DataMember]
        public IEnumerable<VirtualIP> VirtualIPs { get; set; }

        [DataMember(Name = "created")]
        public DateTime CreatedDate { get; set; }

        [DataMember(Name = "updated")]
        public DateTime UpdatedDate { get; set; }
    }

    [DataContract]
    public class LoadBalancer : SimpleLoadBalancer
    {
        [DataMember]
        public int Timeout { get; set; }

        [DataMember]
        public LoadBalancerConnectionLogging ConnectionLogging { get; set; }

        [DataMember]
        public LoadBalancerSessionPersistance SessionPersistance { get; set; }

        [DataMember]
        public LoadBalancerConnectionThrottle ConnectionThrottle { get; set; }

        [DataMember]
        public LoadBalancerCluster Cluster { get; set; }

        [DataMember]
        public LoadBalancerSourceAddresses SourceAddresses { get; set; }
    }

    [DataContract]
    public class LoadBalancerConnectionLogging
    {
        [DataMember]
        public bool Enabled { get; set; }
    }

    [DataContract]
    public class LoadBalancerSessionPersistance
    {
        [DataMember(Name = "persistenceType")]
        public string Type { get; set; }

    }

    [DataContract]
    public class LoadBalancerConnectionThrottle
    {
        [DataMember(Name = "minConnections")]
        public int MinimalConnections { get; set; }

        [DataMember(Name = "maxConnections")]
        public int MaximumConnections { get; set; }

        [DataMember(Name = "maxConnectionRate")]
        public int MaximumConnectionRate { get; set; }

        [DataMember(Name = "rateInterval")]
        public int RateInterval { get; set; }
    }

    public class LoadBalancerCluster
    {
        public string Name { get; set; }
    }

    public class LoadBalancerSourceAddresses
    {
        [DataMember(Name = "ipv6Public")]
        public string PublicIPv6 { get; set; }

        [DataMember(Name = "ipv4Servicenet")]
        public string ServiceNetIPv4 { get; set; }

        [DataMember(Name = "ipv4Public")]
        public string PublicIPv4 { get; set; }
    }
}
