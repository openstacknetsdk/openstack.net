using System;
using System.Collections.Generic;
using System.Linq;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;
using net.openstack.Core.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// <para>The Cloud Load Balancer Provider enables simple access go the Rackspace Cloud Load Balancer services API.
    /// <para />
    /// <para>Documentation URL: http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Overview-d1e82.html</para>
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
            : this(null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudLoadBalancerProvider(CloudIdentity identity)
            : this(identity, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudLoadBalancerProvider(IRestService restService)
            : this(null, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudLoadBalancerProvider(IIdentityProvider identityProvider)
            : this(null, identityProvider, null) { }

                        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// /<param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudLoadBalancerProvider(CloudIdentity identity, IIdentityProvider identityProvider)
            : this(identity, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudLoadBalancerProvider(CloudIdentity identity, IRestService restService)
            : this(identity, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudLoadBalancerProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public  CloudLoadBalancerProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService)
           : base(identity, identityProvider, restService) { }

        //internal CloudLoadBalancerProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService, IJsonObjectMapper<Network> networkResponseMapper)
        //    : base(identity, identityProvider, restService)
        //{
        //    _networkResponseMapper = networkResponseMapper;
        //}

        #endregion

        #region

        /// <inheritdoc />
        public IEnumerable<SimpleLoadBalancer> ListLoadBalancers(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/loadbalancers", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListLoadBalancersResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return BuildCloudServersProviderAwareObject<SimpleLoadBalancer>(response.Data.LoadBalancers, region, identity);
        }

        /// <inheritdoc />
        public LoadBalancer GetLoadBalancer(string id, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/loadbalancers/{1}", GetServiceEndpoint(identity, region), id));

            var response = ExecuteRESTRequest<LoadBalancerResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return BuildCloudServersProviderAwareObject<LoadBalancer>(response.Data.LoadBalancer, region, identity);
        }

        /// <inheritdoc />
        public LoadBalancer CreateLoadBalancer(string name, int port, string protocol, IEnumerable<string> virtualIps, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/loadbalancers", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<LoadBalancerResponse>(identity, urlPath, HttpMethod.POST, 
                body: new CreateLoadBalancerRequest
                {
                    LoadBalancer = new NewLoadBalancer
                    {
                        Name = name,
                        Port = port,
                        Protocol = protocol,
                        VirtualIPs = (virtualIps == null) ? null : virtualIps.Select(v => new NewVirtualIP {Type = v}),
                    }
                });

            if (response == null || response.Data == null)
                return null;

            return BuildCloudServersProviderAwareObject<LoadBalancer>(response.Data.LoadBalancer, region, identity);
        }

        #endregion

        #region Private Members

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudLoadBalancers", region);
        }

        protected override ICloudLoadBalancerProvider BuildProvider(CloudIdentity identity)
        {
            return new CloudLoadBalancerProvider(identity, IdentityProvider, RestService);
        }

        #endregion
    }
}
