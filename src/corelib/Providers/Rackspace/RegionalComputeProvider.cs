using System;
using System.Collections.Generic;
using SimpleRestServices.Client;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Mapping;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Objects.Mapping;

namespace net.openstack.Providers.Rackspace
{
    internal class RegionalComputeProvider : ProviderBase, IComputeProvider
    {
        #region Private/Protected instance members

        private readonly IJsonObjectMapper<Metadata> _metadataMapper;
        private readonly IJsonObjectMapper<CreateServerRequest> _createServerRequestMapper;

        #endregion

        public RegionalComputeProvider(IIdentityProvider identityProvider, IRestService restService) 
            : this(identityProvider, restService, new MetaDataJsonMapper(), new CreateServerRequestJsonMapper()) {}

        public RegionalComputeProvider(IIdentityProvider identityProvider, IRestService restService, IJsonObjectMapper<Metadata> metadataMapper, IJsonObjectMapper<CreateServerRequest> createServerRequestMapper)
            : base(null, identityProvider, restService)
        {
            _metadataMapper = metadataMapper;
            _createServerRequestMapper = createServerRequestMapper;
        }

        public IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers", GetServiceEndpoint(identity)));

            var response = ExecuteRESTRequest<ListServersResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Servers;
        }

        public IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/detail", GetServiceEndpoint(identity)));
            
            var response = ExecuteRESTRequest<ListServersResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Servers;
        }

        public Metadata ListMetadata(string cloudServerId, CloudIdentity identity)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata", GetServiceEndpoint(identity), cloudServerId)); 
            
            var response = ExecuteRESTRequest<MetaDataResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null)
                return null;

            return _metadataMapper.FromJson(response.RawBody);
        }

        public ServerDetails GetDetails(string cloudServerId, CloudIdentity identity)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity), cloudServerId)); 
            
            var response = ExecuteRESTRequest<ServerDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            response.Data.Server.Metadata = _metadataMapper.FromJson(response.RawBody);
            return response.Data.Server;
        }

        public NewServer CreateServer(string cloudServerName, string friendlyName, string imageName, string flavor, CloudIdentity identity)
        {
            var urlPath = new Uri(string.Format("{0}/servers", GetServiceEndpoint(identity)));

            var requestJson = _createServerRequestMapper.ToJson(new CreateServerRequest(){DiskConfig = "AUTO", Flavor = flavor, ImageName = imageName, Name = cloudServerName, FriendlyName = friendlyName});
            var response = ExecuteRESTRequest<CreateServerResponse>(urlPath, HttpMethod.POST, requestJson, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return null; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);
            
            return response.Data.Server;
        }

        public bool UpdateServer(string cloudServerId, CloudIdentity identity, string name, string ipV4Address, string ipV6Address)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity), cloudServerId));

            var requestJson = new UpdateServerRequest(name, ipV4Address, ipV6Address);
            var response = ExecuteRESTRequest<ServerDetailsResponse>(urlPath, HttpMethod.PUT, requestJson, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return false;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        public bool DeleteServer(string cloudServerId, CloudIdentity identity)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity), cloudServerId));

            var defaultSettings = BuildDefaultRequestSettings(new [] {404});
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity, requestSettings: defaultSettings);

            if (response.StatusCode != 200 && response.StatusCode != 204)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        protected string GetServiceEndpoint(CloudIdentity identity)
        {
            return base.GetServiceEndpoint("cloudServersOpenStack", identity);
        }
    }

    internal class ListServersResponse
    {
        public ServerDetails[] Servers { get; set; }
    }
}
