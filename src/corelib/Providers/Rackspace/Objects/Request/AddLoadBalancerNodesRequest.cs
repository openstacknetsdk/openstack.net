using System.Collections.Generic;
using Newtonsoft.Json;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    /// <summary>
    /// This models the JSON request used for the Add Load Balancer Nodes request.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Add_Nodes-d1e2379.html">Add Load Balancer Nodes (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class AddLoadBalancerNodesRequest
    {
        /// <summary>
        /// Gets information about the load balancer node.
        /// </summary>
        [JsonProperty("nodes")]
        public IEnumerable<LoadBalancerNode> Nodes { get; set; } 
    }
}