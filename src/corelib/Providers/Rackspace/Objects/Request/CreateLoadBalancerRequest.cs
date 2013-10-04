using Newtonsoft.Json;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    /// <summary>
    /// This models the JSON request used for the Create Load Balancer request.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Create_Load_Balancer-d1e1635.html">Create Load Balancer (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateLoadBalancerRequest
    {
        /// <summary>
        /// Gets information about the newly created load balancer.
        /// </summary>
        [JsonProperty("loadBalancer")]
        public NewLoadBalancer LoadBalancer { get; set; }
    }
}
