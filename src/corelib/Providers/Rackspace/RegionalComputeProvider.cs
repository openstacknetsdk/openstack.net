using System;
using System.Collections.Generic;
using SimpleRestServices.Client;
using SimpleRestServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Domain.Mapping;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Objects.Mapping;

namespace net.openstack.Providers.Rackspace
{
    class RegionalComputeProvider : ProviderBase, IComputeProvider
    {
        #region Private/Protected instance members

        private readonly IJsonObjectMapper<Metadata> _metadataMapper;
        private readonly IJsonObjectMapper<CreateServerRequest> _createServerRequestMapper;

        #endregion

        public RegionalComputeProvider(Uri urlBase)
            : this(urlBase, new IdentityProvider(),  new JsonRestServices()) {}

        public RegionalComputeProvider(Uri urlBase, IIdentityProvider identityProvider, IRestService restService) 
            : this(urlBase, identityProvider, restService, new MetaDataJsonMapper(), new CreateServerRequestJsonMapper()) {}

        public RegionalComputeProvider(Uri urlBase, IIdentityProvider identityProvider, IRestService restService, IJsonObjectMapper<Metadata> metadataMapper, IJsonObjectMapper<CreateServerRequest> createServerRequestMapper)
            : base(urlBase, identityProvider, restService)
        {
            _metadataMapper = metadataMapper;
            _createServerRequestMapper = createServerRequestMapper;
        }

        public Metadata ListMetadata(string apiServerId, CloudIdentity identity)
        {
            var urlFormat = Settings.GetMetadataUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "cloudDDIAccout", identity.AccountId }, { "apiServerID", apiServerId } });
            var response = ExecuteRESTRequest<MetaDataResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null)
                return null;

            return _metadataMapper.FromJson(response.RawBody);
        }

        public ServerDetails GetDetails(string apiServerId, CloudIdentity identity)
        {
            var urlFormat = Settings.GetDetailsUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "cloudDDIAccout", identity.AccountId }, { "apiServerID", apiServerId } });
            var response = ExecuteRESTRequest<ServerDetailsResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            response.Data.Server.Metadata = _metadataMapper.FromJson(response.RawBody);
            return response.Data.Server;
        }

        public NewServer CreateServer(string cloudServerName, string friendlyName, string imageName, string flavor, CloudIdentity identity)
        {
            var urlFormat = Settings.CreateServerUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "cloudDDIAccout", identity.AccountId }});

            var requestJson = _createServerRequestMapper.ToJson(new CreateServerRequest(){DiskConfig = "AUTO", Flavor = flavor, ImageName = imageName, Name = cloudServerName, FriendlyName = friendlyName});
            var response = ExecuteRESTRequest<CreateServerResponse>(urlPath, HttpMethod.POST, requestJson, identity);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return null; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);
            
            return response.Data.Server;
        }

        public bool DeleteServer(string cloudServerId, CloudIdentity identity)
        {
            var urlFormat = Settings.DeleteServerUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "cloudDDIAccout", identity.AccountId }, { "cloudServerID", cloudServerId } });

            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity);

            if (response.StatusCode != 200 && response.StatusCode != 204)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }
    }
}
