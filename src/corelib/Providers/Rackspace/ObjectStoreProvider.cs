using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class ObjectStoreProvider : ProviderBase, IObjectStoreProvider
    {
        private readonly IObjectStoreValidator _objectStoreValidator;
        #region Constructors

        public ObjectStoreProvider()
            : this(new IdentityProvider(), new JsonRestServices(), new ObjectStoreValidator()) { }

        public ObjectStoreProvider(IIdentityProvider identityProvider, IRestService restService, IObjectStoreValidator objectStoreValidator)
            : base(identityProvider, restService)
        {
            _objectStoreValidator = objectStoreValidator;
        }


        #endregion

        #region Containers

        public IEnumerable<Container> ListContainers(CloudIdentity identity, int? limit = null, string marker = null, string markerEnd = null, string format = "json", string region = null)
        {
            var urlPath = new Uri(string.Format("{0}", GetServiceEndpoint(identity, region)));

            var queryStringParameter = new Dictionary<string, string>();
            queryStringParameter.Add("format", format);

            if (limit != null)
                queryStringParameter.Add("limit", limit.ToString());

            if (!string.IsNullOrWhiteSpace(marker))
                queryStringParameter.Add("marker", marker);

            if (!string.IsNullOrWhiteSpace(markerEnd))
                queryStringParameter.Add("end_marker", markerEnd);
            
            var response = ExecuteRESTRequest<Container []>(identity, urlPath, HttpMethod.GET, null, queryStringParameter);

            if (response == null || response.Data == null)
                return null;

            return response.Data;

        }

        public ObjectStore CreateContainer(CloudIdentity identity, string container, string region = null)
        {
            _objectStoreValidator.ValidateContainerName(container);
            var urlPath = new Uri(string.Format("{0}/{1}", GetServiceEndpoint(identity, region), container));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT);

            if (response.StatusCode == 201)
                return ObjectStore.ContainerCreated;
            if (response.StatusCode == 202)
                return ObjectStore.ContainerExists;

            return ObjectStore.Unknown;
        }

        #endregion

        #region Private methods

        protected string GetServiceEndpoint(CloudIdentity identity, string region = null)
        {
            return base.GetServiceEndpoint(identity, "cloudFiles", region);
        }

        #endregion
    }
}
