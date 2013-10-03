using Newtonsoft.Json;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Rackspace;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    /// <summary>
    /// This models the JSON response used for the Get Load Balancer Details and Create Load Balancer requests.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/List_Load_Balancer_Details-d1e1522.html">Get Load Balancer Details (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Create_Load_Balancer-d1e1635.html">Create Load Balancer (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class LoadBalancerResponse
    {
        /// <summary>
        /// Gets information about the load balancer.
        /// </summary>
        [JsonProperty("loadBalancer")]
        public LoadBalancer LoadBalancer { get; set; }
    }
}