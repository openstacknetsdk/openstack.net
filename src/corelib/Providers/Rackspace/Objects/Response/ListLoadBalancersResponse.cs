using System.Collections.Generic;
using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListLoadBalancersResponse
    {
        [DataMember(Name = "loadBalancers")]
        public IEnumerable<SimpleLoadBalancer> LoadBalancers { get; set; }
    }
}
