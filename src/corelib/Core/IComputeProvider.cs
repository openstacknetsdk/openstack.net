using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IComputeProvider
    {
        // Servers
        IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null);
        IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null);
        NewServer CreateServer(CloudIdentity identity, string cloudServerName, string imageName, string flavor, string diskConfig = null, Metadata metadata = null, string region = null);
        ServerDetails GetDetails(CloudIdentity identity, string cloudServerId, string region = null);
        bool UpdateServer(CloudIdentity identity, string cloudServerId, string name = null, string ipV4Address = null, string ipV6Address = null, string region = null);
        bool DeleteServer(CloudIdentity identity, string cloudServerId, string region = null);
        
        // Server Addresses
        ServerAddresses ListAddresses(CloudIdentity identity, string serverId, string region = null);
        Network ListAddressesByNetwork(CloudIdentity identity, string serverId, string network, string region = null);
        
        // Server Actions
        bool ChangeAdministratorPassword(CloudIdentity identity, string serverId, string password, string region = null);
        bool RebootServer(CloudIdentity identity, string serverId, RebootType rebootType, string region = null);
        ServerDetails RebuildServer(CloudIdentity identity, string serverId, string serverName, string imageName, string flavor, string adminPassword, string ipV4Address = null, string ipV6Address = null, Metadata metadata = null, string diskConfig = null, Personality personality = null,  string region = null);
        bool ResizeServer(CloudIdentity identity, string serverId, string serverName, string flavor, string diskConfig = null, string region = null);
        bool ConfirmServerResize(CloudIdentity identity, string serverId, string region = null);
        bool RevertServerResize(CloudIdentity identity, string serverId, string region = null);
        string RescueServer(CloudIdentity identity, string serverId, string region = null);
        ServerDetails UnRescueServer(CloudIdentity identity, string serverId, string region = null);
        bool CreateImage(CloudIdentity identity, string serverId, string imageName, Metadata metadata = null, string region = null);

        // Volume Attachment Actions

        // Flavors
        IEnumerable<Flavor> ListFlavors(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null);
        IEnumerable<FlavorDetails> ListFlavorsWithDetails(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null);
        FlavorDetails GetFlavor(CloudIdentity identity, string id, string region = null);
            
        // Images
        IEnumerable<ServerImage> ListImages(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null);
        IEnumerable<ServerImageDetails> ListImagesWithDetails(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null);
        ServerImageDetails GetImage(CloudIdentity identity, string imageId, string region = null);
        bool DeleteImage(CloudIdentity identity, string imageId, string region = null);

        // Server metadata
        Metadata ListServerMetadata(CloudIdentity identity, string cloudServerId, string region = null);
        bool SetServerMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null);
        bool UpdateServerMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null);
        string GetServerMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null);
        bool SetServerMetadataItem(CloudIdentity identity, string cloudServerId, string key, string value, string region = null);
        bool DeleteServerMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null);
    
        // Image metadata
        Metadata ListImageMetadata(CloudIdentity identity, string cloudServerId, string region = null);
        bool SetImageMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null);
        bool UpdateImageMetadata(CloudIdentity identity, string cloudServerId, Metadata metadata, string region = null);
        string GetImageMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null);
        bool SetImageMetadataItem(CloudIdentity identity, string cloudServerId, string key, string value, string region = null);
        bool DeleteImageMetadataItem(CloudIdentity identity, string cloudServerId, string key, string region = null);
    }
}
    