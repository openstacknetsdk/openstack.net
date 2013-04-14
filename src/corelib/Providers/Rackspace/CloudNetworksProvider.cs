using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    public class CloudNetworksProvider : ProviderBase, ICloudNetworksProvider
    {
        private readonly int[] _validResponseCode = new[] { 200, 201, 202 };

        #region Constructors

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        public CloudNetworksProvider()
            : this(null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudNetworksProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object.<remarks>[Optional]: If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudNetworksProvider(CloudIdentity identity)
            : base(identity, new CloudIdentityProvider(), new JsonRestServices()) { }

        #endregion


        #region Networks

        public IEnumerable<CloudNetwork> ListNetworks(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListCloudNetworksResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Networks;
        }


        public CloudNetwork CreateNetwork(string cidr, string label, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2", GetServiceEndpoint(identity, region)));
            var cloudNetworkRequest = new CreateCloudNetworkRequest { Details = new CreateCloudNetworksDetails { Cidr = cidr, Label = label } };

            var response = ExecuteRESTRequest<CloudNetworkResponse>(identity, urlPath, HttpMethod.POST, cloudNetworkRequest);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Network;
        }

        public CloudNetwork ShowNetwork(string network_id, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2/{1}", GetServiceEndpoint(identity, region), network_id));
            var response = ExecuteRESTRequest<CloudNetworkResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Network;
        }

        public bool DeleteNetwork(string network_id, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/os-networksv2/{1}", GetServiceEndpoint(identity, region), network_id));

            Response response = null;
            try
            {
                response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);                
            } 
            catch(UserNotAuthorizedException ex)
            {
                if(ex.Response.StatusCode == 403)
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

        #endregion

    }
}
