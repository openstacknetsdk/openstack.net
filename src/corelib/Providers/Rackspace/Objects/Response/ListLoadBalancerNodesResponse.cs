using System.Collections.Generic;
using Newtonsoft.Json;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    /// <summary>
    /// This models the JSON response used for the List Load Balancer Nodes request.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/List_Nodes-d1e2218.html">List Load Balancer Nodes (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class ListLoadBalancerNodesResponse
    {
        /// <summary>
        /// Gets a collection of information about the load balancer nodes.
        /// </summary>
        [JsonProperty("nodes")]
        public IEnumerable<LoadBalancerNode> Nodes { get; set; }
    }
}