using System.Collections.Generic;
using Newtonsoft.Json;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    /// <summary>
    /// This models the JSON response used for the List Load Balancers request.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/List_Load_Balancers-d1e1367.html">List Load Balancer Nodes (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class ListLoadBalancersResponse
    {
        /// <summary>
        /// Gets a collection of information about the load balancers.
        /// </summary>
        [JsonProperty("loadBalancers")]
        public IEnumerable<SimpleLoadBalancer> LoadBalancers { get; set; }
    }
}
