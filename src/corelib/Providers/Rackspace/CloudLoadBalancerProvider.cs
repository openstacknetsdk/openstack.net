using System;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Rackspace;
using net.openstack.Core.Providers;
using net.openstack.Core.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// <para>The Cloud Load Balancer Provider enables simple access go the Rackspace Cloud Load Balancer services API.</para>  
    /// <para />
    /// <para>Documentation URL: http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Overview-d1e82.html </para>
    /// </summary>
    /// <see cref="ICloudLoadBalancerProvider"/>
    /// <inheritdoc />
    public class CloudLoadBalancerProvider : ProviderBase<ICloudLoadBalancerProvider>, ICloudLoadBalancerProvider
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        public CloudLoadBalancerProvider()
            : this(null, null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudLoadBalancerProvider(CloudIdentity identity)
            : this(identity, null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudLoadBalancerProvider(IRestService restService)
            : this(null, null, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudLoadBalancerProvider(IIdentityProvider identityProvider)
            : this(null, null,  identityProvider, null) { }

                        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// /<param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudLoadBalancerProvider(CloudIdentity identity, IIdentityProvider identityProvider)
            : this(identity, null, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudLoadBalancerProvider(CloudIdentity identity, IRestService restService)
            : this(identity, null, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="defaultRegion">The default region to use for calls that do not explicitly specify a region. If this value is <c>null</c>, the default region for the user will be used; otherwise if the service uses region-specific endpoints all calls must specify an explicit region.</param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public  CloudLoadBalancerProvider(CloudIdentity identity, string defaultRegion, IIdentityProvider identityProvider, IRestService restService)
           : base(identity, defaultRegion, identityProvider, restService) { }

        #endregion

        #region Load Balancers

        /// <inheritdoc />
        public IEnumerable<SimpleLoadBalancer> ListLoadBalancers(string region = null, CloudIdentity identity = null)
        {
            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}/loadbalancers", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListLoadBalancersResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return BuildCloudServersProviderAwareObject<SimpleLoadBalancer>(response.Data.LoadBalancers, region, identity);
        }

        /// <inheritdoc />
        public LoadBalancer GetLoadBalancer(string id, string region = null, CloudIdentity identity = null)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id cannot be empty");

            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}/loadbalancers/{1}", GetServiceEndpoint(identity, region), id));

            var response = ExecuteRESTRequest<LoadBalancerResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return BuildCloudServersProviderAwareObject<LoadBalancer>(response.Data.LoadBalancer, region, identity);
        }

        /// <inheritdoc />
        public LoadBalancer CreateLoadBalancer(string name, LoadBalancerProtocol protocol, IEnumerable<VirtualIPType> virtualIps, IEnumerable<LoadBalancerNode> nodes, string region = null, CloudIdentity identity = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (protocol == null)
                throw new ArgumentNullException("protocol");
            if (virtualIps == null)
                throw new ArgumentNullException("virtualIps");
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");
            if (string.IsNullOrEmpty(protocol.Name))
                throw new ArgumentException("Protocol name cannot be empty");
            if (!virtualIps.Any())
                throw new ArgumentException("Atleast 1 virtual IP type must be specified");
            if (!nodes.Any(n => n.Condition == LoadBalancerNodeCondition.Enabled))
                throw new ArgumentException("Atleast 1 node with ENABLED condition must be specified");

            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}/loadbalancers", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<LoadBalancerResponse>(identity, urlPath, HttpMethod.POST, 
                new CreateLoadBalancerRequest
                {
                    LoadBalancer = new NewLoadBalancer
                    {
                        Name = name,
                        Port = protocol.Port,
                        Protocol = protocol.Name,
                        VirtualIPs = virtualIps == null ? null : virtualIps.Select(v => new NewLoadBalancerVirtualIP {Type = v.ToString()}),
                        Nodes = nodes
                    }
                });

            if (response == null || response.Data == null)
                return null;

            return BuildCloudServersProviderAwareObject<LoadBalancer>(response.Data.LoadBalancer, region, identity);
        }

        ///// <inheritdoc />
        //public LoadBalancer UpdateLoadBalancer(string id, string name, LoadBalancerProtocol protocol, LoadBalancerAlgorithm algorithm, TimeSpan timeout, bool halfClosed, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        /// <inheritdoc />
        public void RemoveLoadBalancer(string id, string region = null, CloudIdentity identity = null)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id cannot be empty");

            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}/loadbalancers/{1}", GetServiceEndpoint(identity, region), id));

            ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);
        }

        ///// <inheritdoc />
        //public void RemoveLoadBalancers(IEnumerable<string> id, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public LoadBalancer WaitForLoadBalancerState(string serverId, string expectedState, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public LoadBalancer WaitForLoadBalancerState(string serverId, string[] expectedStates, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null,Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public LoadBalancer WaitForLoadBalancerActive(string serverId, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public void WaitForLoadBalancerDeleted(string serverId, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Load Balancer Stats

        ///// <inheritdoc />
        //public LoadBalancerStats GetLoadBalancerStats(string id, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Load Balancer Nodes
        /// <inheritdoc />
        public IEnumerable<LoadBalancerNode> ListLoadBalancerNodes(string loadBalancerId, string region = null, CloudIdentity identity = null)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");

            if (string.IsNullOrEmpty(loadBalancerId))
                throw new ArgumentException("loadBalancerId cannot be empty");

            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}/loadbalancers/{1}/nodes", GetServiceEndpoint(identity, region), loadBalancerId));

            var response = ExecuteRESTRequest<ListLoadBalancerNodesResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Nodes;
        }

        ///// <inheritdoc />
        //public LoadBalancerNode GetLoadBalancerNode(string loadBalancerId, string nodeId, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        /// <inheritdoc />
        public LoadBalancerNode AddLoadBalancerNode(string loadBalancerId, string ipAddress, LoadBalancerNodeCondition condition, int port, LoadBalancerNodeType type, int? weight = null, string region = null, CloudIdentity identity = null)
        {
            if (loadBalancerId == null)
                throw new ArgumentNullException("loadBalancerId");
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");
            if (condition == null)
                throw new ArgumentNullException("condition");
            if (type == null)
                throw new ArgumentNullException("type");

            if (string.IsNullOrEmpty(loadBalancerId))
                throw new ArgumentException("loadBalancerId cannot be empty");
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentException("ipAddress cannot be empty");

            if (port <= 0)
                throw new ArgumentOutOfRangeException("port");

            if (weight.HasValue)
            {
                if(weight.Value < 1 || weight.Value > 100)
                    throw new ArgumentOutOfRangeException("weight");
            }

            CheckIdentity(identity);

            var urlPath = new Uri(string.Format("{0}/loadbalancers/{1}/nodes", GetServiceEndpoint(identity, region), loadBalancerId));

            var body = new AddLoadBalancerNodesRequest
                {
                    Nodes =
                        new[]
                            {
                                new LoadBalancerNode
                                    {
                                        IPAddress = ipAddress,
                                        Condition = condition,
                                        Port = port,
                                        Type = type,
                                        Weight = weight
                                    }
                            }
                };
            var response = ExecuteRESTRequest<ListLoadBalancerNodesResponse>(identity, urlPath, HttpMethod.POST, body: body);

            if (response == null || response.Data == null || response.Data.Nodes == null)
                return null;

            return response.Data.Nodes.FirstOrDefault();
        }

        ///// <inheritdoc />
        //public LoadBalancerNode UpdateLoadBalancerNode(string loadBalancerId, string nodeId, LoadBalancerNodeCondition condition, LoadBalancerNodeType type, int weight, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public LoadBalancerNode RemoveLoadBalancerNode(string loadBalancerId, string nodeId, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public LoadBalancerNode RemoveLoadBalancerNodes(string loadBalancerId, IEnumerable<string> nodeId, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc />
        //public IEnumerable<LoadBalancerNodeServiceEvent> ListLoadBalancerNodeServiceEvents(string loadBalancerId, string nodeId, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Load Balancer Virtual IPs

        ///// <inheritdoc />
        //public IEnumerable<VirtualIP> ListLoadBalancerVirtualIPs(string loadBalancerId, string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Allowed Domains

        ///// <inheritdoc />
        //public IEnumerable<AllowedDomain> ListAllowedDomains(string region = null, CloudIdentity identity = null)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Private Members

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "rax:load-balancer", "cloudLoadBalancers", region);
        }

        #endregion
    }
}
