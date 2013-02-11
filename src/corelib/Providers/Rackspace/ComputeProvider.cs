using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;

namespace net.openstack.Providers.Rackspace
{
    public class ComputeProvider : ProviderBase, IComputeProvider
    {
        #region Constructors
       
        public ComputeProvider()
            : this(new IdentityProvider(), new JsonRestServices()) { }

        public ComputeProvider(IIdentityProvider identityProvider, IRestService restService) 
            : base(identityProvider, restService){}

        #endregion

        #region Servers

        public IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListServersResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Servers;
        }

        public IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/detail", GetServiceEndpoint(identity, region)));
            
            var response = ExecuteRESTRequest<ListServersResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Servers;
        }

        public NewServer CreateServer(CloudIdentity identity, string cloudServerName, string imageName, string flavor, string diskConfig = null, Metadata metadata = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers", GetServiceEndpoint(identity, region)));

            var request = new CreateServerRequest
                              {
                                  Details = new CreateServerDetails
                                                {
                                                    Name = cloudServerName,
                                                    DiskConfig = diskConfig,
                                                    Flavor = flavor,
                                                    ImageName = imageName,
                                                    Metadata = metadata
                                                }
                              };
            var response = ExecuteRESTRequest<CreateServerResponse>(identity, urlPath, HttpMethod.POST, request);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return null; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);
            
            return response.Data.Server;
        }

        public ServerDetails GetDetails(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest<ServerDetailsResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null || response.Data.Server == null)
                return null;

            //response.Data.Server.Metadata = _metadataMapper.From(response.RawBody);
            return response.Data.Server;
        }

        public bool UpdateServer(CloudIdentity identity, string cloudServerId, string name, string ipV4Address, string ipV6Address, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity, region), cloudServerId));

            var requestJson = new UpdateServerRequest(name, ipV4Address, ipV6Address);
            var response = ExecuteRESTRequest<ServerDetailsResponse>(identity, urlPath, HttpMethod.PUT, requestJson);

            if (response == null || response.Data == null || response.Data.Server == null)
                return false;

            if (response.StatusCode != 200 && response.StatusCode != 202)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        public bool DeleteServer(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}", GetServiceEndpoint(identity, region), cloudServerId));

            var defaultSettings = BuildDefaultRequestSettings(new [] {404});
            var response = ExecuteRESTRequest<object>(identity, urlPath, HttpMethod.DELETE, requestSettings: defaultSettings);

            if (response.StatusCode != 200 && response.StatusCode != 204)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        public ServerDetails WaitForServerState(CloudIdentity identity, string cloudServerId, string expectedState, string[] errorStates, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400)
        {
            var serverDetails = GetDetails(identity, cloudServerId, region);

            int count = 0;
            while (!serverDetails.Status.Equals(expectedState) && !errorStates.Contains(serverDetails.Status) && count < refreshCount)
            {
                Thread.Sleep(refreshDelayInMS);
                serverDetails = GetDetails(identity, cloudServerId, region);
                count++;
            }

            if (errorStates.Contains(serverDetails.Status))
                throw new ServerEnteredErrorStateException(serverDetails.Status);

            return serverDetails;
        }

        public ServerDetails WaitForServerActive(CloudIdentity identity, string cloudServerId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400)
        {
            return WaitForServerState(identity, cloudServerId, ServerState.ACTIVE, new [] {ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED}, region, refreshCount, refreshDelayInMS);
        }

        public ServerDetails WaitForServerDeleted(CloudIdentity identity, string cloudServerId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400)
        {
            return WaitForServerState(identity, cloudServerId, ServerState.DELETED, new[] { ServerState.ERROR, ServerState.UNKNOWN, ServerState.SUSPENDED }, region, refreshCount, refreshDelayInMS);
        }

        #endregion

        #region Server Addresses

        public ServerAddresses ListAddresses(CloudIdentity identity, string serverId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/ips", GetServiceEndpoint(identity, region), serverId));

            var response = ExecuteRESTRequest<ListAddressesResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Addresses;
        }

        public Network ListAddressesByNetwork(CloudIdentity identity, string serverId, string network, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/ips/{2}", GetServiceEndpoint(identity, region), serverId, network));

            var response = ExecuteRESTRequest<ListAddressesByNetworkResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Network;
        }

        #endregion

        #region Server Actions
        
        public bool ChangeAdministratorPassword(CloudIdentity identity, string serverId, string password, string region = null)
        {
            var request = new ChangeServerAdminPasswordRequest { Details = new ChangeAdminPasswordDetails { AdminPassword = password } };
            var resp = ExecuteServerAction(identity, serverId, request, region);

            return resp;
        }

        public bool RebootServer(CloudIdentity identity, string serverId, RebootType rebootType, string region = null)
        {
            var request = new ServerRebootRequest {Details = new ServerRebootDetails {Type = rebootType}};
            var resp = ExecuteServerAction(identity, serverId, request, region);

            return resp;
        }

        public ServerDetails RebuildServer(CloudIdentity identity, string serverId, string serverName, string imageName, string flavor, string adminPassword, string ipV4Address = null, string ipV6Address = null, Metadata metadata = null, string diskConfig = null, Personality personality = null, string region = null)
        {
            var request = new ServerRebuildRequest { Details = new ServerRebuildDetails
                                                                   {
                                                                       Name = serverName,
                                                                       ImageName = imageName,
                                                                       Flavor = flavor,
                                                                       DiskConfig = diskConfig,
                                                                       AdminPassword = adminPassword,
                                                                       Metadata = metadata,
                                                                       Personality = personality,
                                                                       AccessIPv4 = ipV4Address,
                                                                       AccessIPv6 = ipV6Address,
                                                                   } };
            var resp = ExecuteServerAction<ServerDetailsResponse>(identity, serverId, request, region);

            return resp.Server;
        }

        public bool ResizeServer(CloudIdentity identity, string serverId, string serverName, string flavor, string diskConfig = null, string region = null)
        {
            var request = new ServerResizeRequest
                {
                    Details = new ServerResizeDetails
                    {
                        Name = serverName,
                        Flavor = flavor,
                        DiskConfig = diskConfig,
                    }
                };
            var resp = ExecuteServerAction(identity, serverId, request, region);

            return resp;
        }

        public bool ConfirmServerResize(CloudIdentity identity, string serverId, string region = null)
        {
            var request = new ConfirmServerResizeRequest();
            var resp = ExecuteServerAction(identity, serverId, request, region);

            return resp;
        }

        public bool RevertServerResize(CloudIdentity identity, string serverId, string region = null)
        {
            var request = new RevertServerResizeRequest();
            var resp = ExecuteServerAction(identity, serverId, request, region);

            return resp;
        }

        public string RescueServer(CloudIdentity identity, string serverId, string region = null)
        {
            var request = new RescueServerRequest{Details = "none"};
            var resp = ExecuteServerAction<RescueServerResponse>(identity, serverId, request, region);

            return resp.AdminPassword;
        }

        public ServerDetails UnRescueServer(CloudIdentity identity, string serverId, string region = null)
        {
            var request = new UnrescueServerRequest { Details = "none" };
            var resp = ExecuteServerAction<ServerDetailsResponse>(identity, serverId, request, region);

            return resp.Server;
        }

        public bool CreateImage(CloudIdentity identity, string serverId, string imageName, Metadata metadata = null, string region = null)
        {
            var request = new CreateServerImageRequest { Details = new CreateServerImageDetails{ImageName = imageName, Metadata = metadata} };
            var resp = ExecuteServerAction(identity, serverId, request, region);

            return resp;
        }

        private readonly int[] _validServerActionResponseCode = new[] { 200, 202, 203 };

        private T ExecuteServerAction<T>(CloudIdentity identity, string serverId, object body, string region = null) where T : new()
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/action", GetServiceEndpoint(identity, region), serverId));

            var response = ExecuteRESTRequest<T>(identity, urlPath, HttpMethod.POST, body);

            if (response == null || response.Data == null || !_validServerActionResponseCode.Contains(response.StatusCode))
                return default(T);

            return response.Data;
        }

        private bool ExecuteServerAction(CloudIdentity identity, string serverId, object body, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/action", GetServiceEndpoint(identity, region), serverId));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, body);

            if (response == null || !_validServerActionResponseCode.Contains(response.StatusCode))
                return false;

            return true;
        }

        #endregion

        #region Volume Attachment Actions

        #endregion

        #region Flavors

        public IEnumerable<Flavor> ListFlavors(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/flavors", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListFlavorsResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Flavors;
        }

        public IEnumerable<FlavorDetails> ListFlavorsWithDetails(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/flavors/detail", GetServiceEndpoint(identity, region)));

            var response = ExecuteRESTRequest<ListFlavorDetailsResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Flavors;
        }

        public FlavorDetails GetFlavor(CloudIdentity identity, string id, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/flavors/{1}", GetServiceEndpoint(identity, region), id));

            var response = ExecuteRESTRequest<FlavorDetailsResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Flavor;
        }

        #endregion

        #region Images

        public IEnumerable<ServerImage> ListImages(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = new DateTime(), string markerId = null, int limit = 0, string imageType = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images", GetServiceEndpoint(identity, region)));

            var queryStringParameters = BuildListImagesQueryStringParameters(serverId, imageName, imageStatus, changesSince, markerId, limit, imageType);

            var response = ExecuteRESTRequest<ListImagesResponse>(identity, urlPath, HttpMethod.GET, queryStringParameter: queryStringParameters);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Images;
        }

        public IEnumerable<ServerImageDetails> ListImagesWithDetails(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/detail", GetServiceEndpoint(identity, region)));

            var queryStringParameters = BuildListImagesQueryStringParameters(serverId, imageName, imageStatus, changesSince, markerId, limit, imageType);

            var response = ExecuteRESTRequest<ListImagesDetailsResponse>(identity, urlPath, HttpMethod.GET, queryStringParameter: queryStringParameters);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Images;
        }

        private Dictionary<string, string> BuildListImagesQueryStringParameters(string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null)
        {
            var queryParameters = new Dictionary<string, string>();

            if(!string.IsNullOrWhiteSpace(serverId))
                queryParameters.Add("server", serverId);

            if (!string.IsNullOrWhiteSpace(imageName))
                queryParameters.Add("name", imageName);

            if (!string.IsNullOrWhiteSpace(imageStatus))
                queryParameters.Add("status", imageStatus);

            if (changesSince != default(DateTime))
                queryParameters.Add("changes-since", changesSince.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (!string.IsNullOrWhiteSpace(markerId))
                queryParameters.Add("marker", markerId);

            if (limit > 0)
                queryParameters.Add("limit", limit.ToString());

            if(!string.IsNullOrWhiteSpace(imageType))
                queryParameters.Add("type", imageType);

            return queryParameters;
        } 

        public ServerImageDetails GetImage(CloudIdentity identity, string imageId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}", GetServiceEndpoint(identity, region), imageId));

            var response = ExecuteRESTRequest<GetImageDetailsResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Image;
        }

        public bool DeleteImage(CloudIdentity identity, string imageId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}", GetServiceEndpoint(identity, region), imageId));

            var defaultSettings = BuildDefaultRequestSettings(new[] { 404 });
            var response = ExecuteRESTRequest<object>(identity, urlPath, HttpMethod.DELETE, requestSettings: defaultSettings);

            if (response.StatusCode != 200 && response.StatusCode != 203)
                return false; // throw new ExternalServiceException(response.StatusCode, response.Status, response.RawBody);

            return true;
        }

        public ServerImageDetails WaitForImageState(CloudIdentity identity, string imageId, string expectedState, string[] errorStates, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400)
        {
            var details = GetImage(identity, imageId, region);

            int count = 0;
            while (!details.Status.Equals(expectedState) && !errorStates.Contains(details.Status) && count < refreshCount)
            {
                Thread.Sleep(refreshDelayInMS);
                details = GetImage(identity, imageId, region);
                count++;
            }

            if (errorStates.Contains(details.Status))
                throw new ServerEnteredErrorStateException(details.Status);

            return details;
        }

        public ServerImageDetails WaitForImageActive(CloudIdentity identity, string imageId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400)
        {
            return WaitForImageState(identity, imageId, ImageState.ACTIVE, new[] { ImageState.ERROR, ImageState.UNKNOWN, ImageState.SUSPENDED }, region, refreshCount, refreshDelayInMS);
        }

        public ServerImageDetails WaitForImageDeleted(CloudIdentity identity, string imageId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400)
        {
            return WaitForImageState(identity, imageId, ImageState.DELETED, new[] { ImageState.ERROR, ImageState.UNKNOWN, ImageState.SUSPENDED }, region, refreshCount, refreshDelayInMS);
        }

        #endregion

        #region Server Metadata

        public Metadata ListServerMetadata(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest<MetaDataResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null)
                return null;

            return response.Data.Metadata;
        }

        public bool SetServerMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, new UpdateMetadataRequest { Metadata = metadata });

            if (response.StatusCode == 200)
                return true;

            return false;
        }

        public bool UpdateServerMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, new UpdateMetadataRequest { Metadata = metadata });

            if (response.StatusCode == 200)
                return true;

            return false;
        }

        public string GetServerMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata/{2}", GetServiceEndpoint(identity, region), cloudServerId, key));

            var response = ExecuteRESTRequest<MetaDataResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || (response.StatusCode != 200 && response.StatusCode != 203) || response.Data == null || response.Data.Metadata == null || response.Data.Metadata.Count == 0)
                return null;

            return response.Data.Metadata.First().Value;
        }

        public bool SetServerMetadataItem(CloudIdentity identity, string cloudServerId, string key, string value, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata/{2}", GetServiceEndpoint(identity, region), cloudServerId, key));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, new UpdateMetadataItemRequest { Metadata = new Metadata {{key, value}} });

            if (response.StatusCode == 200)
                return true;

            return false;
        }

        public bool DeleteServerMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/servers/{1}/metadata/{2}", GetServiceEndpoint(identity, region), cloudServerId, key));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Image Metadata

        public Metadata ListImageMetadata(CloudIdentity identity, string cloudServerId, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest<MetaDataResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null)
                return null;

            return response.Data.Metadata;
        }

        public bool SetImageMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, new UpdateMetadataRequest { Metadata = metadata });

            if (response.StatusCode == 200)
                return true;

            return false;
        }

        public bool UpdateImageMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}/metadata", GetServiceEndpoint(identity, region), cloudServerId));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, new UpdateMetadataRequest { Metadata = metadata });

            if (response.StatusCode == 200)
                return true;

            return false;
        }

        public string GetImageMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}/metadata/{2}", GetServiceEndpoint(identity, region), cloudServerId, key));

            var response = ExecuteRESTRequest<MetaDataResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null || response.Data.Metadata == null || response.Data.Metadata.Count == 0)
                return null;

            return response.Data.Metadata.First().Value;
        }

        public bool SetImageMetadataItem(CloudIdentity identity, string cloudServerId, string key, string value, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}/metadata/{2}", GetServiceEndpoint(identity, region), cloudServerId, key));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.PUT, new UpdateMetadataItemRequest { Metadata = new Metadata { { key, value } } });

            if (response.StatusCode == 200)
                return true;

            return false;
        }
        
        public bool DeleteImageMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null)
        {
            var urlPath = new Uri(string.Format("{0}/images/{1}/metadata/{2}", GetServiceEndpoint(identity, region), cloudServerId, key));

            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            if (response.StatusCode == 204)
                return true;

            return false;
        }

        #endregion

        #region Private methods
        
        protected string GetServiceEndpoint(CloudIdentity identity, string region = null)
        {
            return base.GetServiceEndpoint(identity, "cloudServersOpenStack", region);
        }

        #endregion
    }

    public class ServerEnteredErrorStateException : Exception
    {
        public string Status { get; private set; }

        public ServerEnteredErrorStateException(string status) : base(string.Format("The server entered an error state: '{0}'", status))
        {
            Status = status;
        }
    }
}
