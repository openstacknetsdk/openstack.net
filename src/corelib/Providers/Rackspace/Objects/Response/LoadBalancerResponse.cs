using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class LoadBalancerResponse
    {
        [DataMember(Name = "loadBalancer")]
        public LoadBalancer LoadBalancer { get; set; }
    }
}