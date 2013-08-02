﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// <para>The Cloud Networks Provider enable simple access to the Rackspace Cloud Network Services.
    /// Cloud Networks lets you create a virtual Layer 2 network, known as an isolated network, 
    /// which gives you greater control and security when you deploy web applications.</para>
    /// <para />
    /// <para>Documentation URL: http://docs.rackspace.com/servers/api/v2/cn-gettingstarted/content/ch_overview.html</para>
    /// </summary>
    /// <see cref="INetworksProvider"/>
    /// <inheritdoc />
    public class CloudNetworksProvider : ProviderBase<INetworksProvider>, INetworksProvider
    {
        private readonly HttpStatusCode[] _validResponseCode = new[] { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted };

        #region Constructors

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        public CloudNetworksProvider()
            : this(null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudNetworksProvider(CloudIdentity identity)
            : this(identity, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudNetworksProvider(IRestService restService)
            : this(null, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudNetworksProvider(IIdentityProvider identityProvider)
            : this(null, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// /<param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudNetworksProvider(CloudIdentity identity, IIdentityProvider identityProvider)
            : this(identity, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudNetworksProvider(CloudIdentity identity, IRestService restService)
            : this(identity, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public  CloudNetworksProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService)
            : base(identity, identityProvider, restService) { }

        #endregion


        #region Networks

        /// <inheritdoc />
        public IEnumerable<CloudNetwork> ListNetworks(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListCloudNetworksResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Networks;
        }

        /// <inheritdoc />
        public CloudNetwork CreateNetwork(string cidr, string label, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2", GetServiceEndpoint(identity, region)));
            var cloudNetworkRequest = new CreateCloudNetworkRequest { Details = new CreateCloudNetworksDetails { Cidr = cidr, Label = label } };

            var response = ExecuteRESTRequest<CloudNetworkResponse>(identity, urlPath, HttpMethod.POST, cloudNetworkRequest);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Network;
        }

        /// <inheritdoc />
        public CloudNetwork ShowNetwork(string networkId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2/{1}", GetServiceEndpoint(identity, region), networkId));
            var response = ExecuteRESTRequest<CloudNetworkResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Network;
        }

        /// <inheritdoc />
        public bool DeleteNetwork(string networkId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2/{1}", GetServiceEndpoint(identity, region), networkId));

            Response response = null;
            try
            {
                response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);                
            } 
            catch(UserNotAuthorizedException ex)
            {
                if(ex.Response.StatusCode == HttpStatusCode.Forbidden)
                    throw new UserAuthorizationException("ERROR: Cannot delete network. Ensure that all servers are removed from this network first.");
            }

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        #endregion


        #region Private methods

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "cloudServersOpenStack", region);
        }

        protected override INetworksProvider BuildProvider(CloudIdentity identity)
        {
            return new CloudNetworksProvider(identity, IdentityProvider, RestService);
        }

        #endregion

        
    }
}
