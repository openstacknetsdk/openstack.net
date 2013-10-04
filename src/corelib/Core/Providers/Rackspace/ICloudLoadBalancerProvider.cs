using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Rackspace;
using net.openstack.Core.Exceptions.Response;

namespace net.openstack.Core.Providers.Rackspace
{
    /// <summary>
    /// Describes the available Cloud Load Balancer actions and services
    /// </summary>
    public interface ICloudLoadBalancerProvider
    {

        /// <summary>
        /// Retrieves a list of basic information for load balancers in the account
        /// </summary>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of <see cref="SimpleLoadBalancer"/> objects describing the requested load balancers.</returns>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/List_Load_Balancers-d1e1367.html">List Load Balancers (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        IEnumerable<SimpleLoadBalancer> ListLoadBalancers(string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Get the details of the specified load balancer.
        /// </summary>
        /// <param name="id">The Id of the specified Load Balancer</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="LoadBalancer"/></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="id"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/List_Load_Balancer_Details-d1e1522.html">List Load Balancers (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        LoadBalancer GetLoadBalancer(string id, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Creates a new Load Balancer.
        /// </summary>
        /// <param name="name">Name of the load balancer to create. The name must be 128 characters or less in length, and all UTF-8 characters are valid. Refer to http://www.utf8-chartable.de/ for information about the UTF-8 character set. Refer to the request examples in this section for the required xml/json format.</param>
        /// <param name="protocol">Protocol of the service which is being load balanced. Refer to Section 4.15, “Protocols” for a table of available protocols.</param>
        /// <param name="virtualIps">Type of virtualIp to add along with the creation of a load balancer.<remarks>Available values: [PUBLIC,SERVICENET]</remarks></param>
        /// <param name="nodes">Type of virtualIp to add along with the creation of a load balancer.<remarks>Available values: [PUBLIC,SERVICENET]</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>A <see cref="LoadBalancer"/> instance containing the details for the newly created load balancer</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para><paramref name="protocol"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="virtualIps"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="nodes"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is empty.
        /// <para>-or-</para>
        /// <para><paramref name="protocol.Name"/> is empty.</para>
        /// <para>-or-</para>
        /// <para><paramref name="virtualIps"/> does not conatin at least one item.</para>
        /// <para>-or-</para>
        /// <para><paramref name="nodes"/> does not conatin at least one item with an ENABLED condition</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Create_Load_Balancer-d1e1635.html">List Load Balancers (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        LoadBalancer CreateLoadBalancer(string name, LoadBalancerProtocol protocol, IEnumerable<VirtualIPType> virtualIps, IEnumerable<LoadBalancerNode> nodes, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Remove a load balancer from the account.
        /// </summary>
        /// <param name="id">The id of the load balancer.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="id"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="id"/> is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Remove_Load_Balancer-d1e2093.html">List Load Balancers (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        void RemoveLoadBalancer(string id, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// List node(s) configured for the load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The id of the load balancer.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>A collection of <see cref="LoadBalancerNode"/> objects describing the requested load balancer nodes.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loadBalancerId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="loadBalancerId"/> is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/List_Nodes-d1e2218.html">List Load Balancers (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        IEnumerable<LoadBalancerNode> ListLoadBalancerNodes(string loadBalancerId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// List node(s) configured for the load balancer.
        /// </summary>
        /// <param name="loadBalancerId">The id of the load balancer.</param>
        /// <param name="ipAddress">IP address or domain name for the node.</param>
        /// <param name="condition">Condition for the node, which determines its role within the load balancer.</param>
        /// <param name="port">Port number for the service you are load balancing.</param>
        /// <param name="type">Type of node to add.</param>
        /// <param name="weight">Weight of node to add. If the specified load balancer is in the <see cref="LoadBalancerAlgorithm.WeightedRoundRobin"/> mode, then the user should assign the relevant weight to the node using the weight attribute for the node. Must be an integer from 1 to 100.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>A <see cref="LoadBalancerNode"/>  instance containing the details for the newly added load balancer node</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loadBalancerId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <paramref name="ipAddress"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <paramref name="condition"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="loadBalancerId"/> is empty.
        /// <para>-or-</para>
        /// <paramref name="ipAddress"/> is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Add_Nodes-d1e2379.html">List Load Balancers (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
        LoadBalancerNode AddLoadBalancerNode(string loadBalancerId, string ipAddress, LoadBalancerNodeCondition condition, int port, LoadBalancerNodeType type, int? weight = null, string region = null, CloudIdentity identity = null);
        
        //LoadBalancerNode GetLoadBalancerNode(string loadBalancerId, string nodeId, string region = null, CloudIdentity identity = null);
        
        //void RemoveLoadBalancers(IEnumerable<string> id, string region = null, CloudIdentity identity = null);
        
        //LoadBalancer UpdateLoadBalancer(string id, string name, LoadBalancerProtocol protocol, LoadBalancerAlgorithm algorithm, TimeSpan timeout, bool halfClosed, string region = null, CloudIdentity identity = null);
        
        //LoadBalancerStats GetLoadBalancerStats(string id, string region = null, CloudIdentity identity = null);
        
        //LoadBalancerNode UpdateLoadBalancerNode(string loadBalancerId, string nodeId, LoadBalancerNodeCondition condition, LoadBalancerNodeType type, int weight,  string region = null, CloudIdentity identity = null);
        
        //LoadBalancerNode RemoveLoadBalancerNode(string loadBalancerId, string nodeId, string region = null, CloudIdentity identity = null);
        
        //LoadBalancerNode RemoveLoadBalancerNodes(string loadBalancerId, IEnumerable<string> nodeId, string region = null, CloudIdentity identity = null);

        //IEnumerable<LoadBalancerNodeServiceEvent> ListLoadBalancerNodeServiceEvents(string loadBalancerId, string nodeId, string region = null, CloudIdentity identity = null);

        //IEnumerable<VirtualIP> ListLoadBalancerVirtualIPs(string loadBalancerId, string region = null, CloudIdentity identity = null);

        //IEnumerable<AllowedDomain> ListAllowedDomains(string region = null, CloudIdentity identity = null); 
            
        //LoadBalancer WaitForLoadBalancerState(string serverId, string expectedState, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        //LoadBalancer WaitForLoadBalancerState(string serverId, string[] expectedStates, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        //LoadBalancer WaitForLoadBalancerActive(string serverId, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

        //void WaitForLoadBalancerDeleted(string serverId, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null);

    }
}
