using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IComputeProvider
    {
        // Servers
        IEnumerable<Server> ListServers(string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null, CloudIdentity identity = null);
        IEnumerable<ServerDetails> ListServersWithDetails(string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null, CloudIdentity identity = null);
        NewServer CreateServer(string cloudServerName, string imageName, string flavor, string diskConfig = null, Metadata metadata = null, bool attachToServiceNetwork = false, bool attachToPublicNetwork = false, string region = null, CloudIdentity identity = null);
        ServerDetails GetDetails(string cloudServerId, string region = null, CloudIdentity identity = null);
        bool UpdateServer(string cloudServerId, string name = null, string ipV4Address = null, string ipV6Address = null, string region = null, CloudIdentity identity = null);
        bool DeleteServer(string cloudServerId, string region = null, CloudIdentity identity = null);

        // Server Addresses
        ServerAddresses ListAddresses(string serverId, string region = null, CloudIdentity identity = null);
        IEnumerable<AddressDetails> ListAddressesByNetwork(string serverId, string network, string region = null, CloudIdentity identity = null);
        
        // Server Actions
        bool ChangeAdministratorPassword(string serverId, string password, string region = null, CloudIdentity identity = null);
        bool RebootServer(string serverId, RebootType rebootType, string region = null, CloudIdentity identity = null);
        ServerDetails RebuildServer(string serverId, string serverName, string imageName, string flavor, string adminPassword, string ipV4Address = null, string ipV6Address = null, Metadata metadata = null, string diskConfig = null, Personality personality = null,  string region = null, CloudIdentity identity = null);
        bool ResizeServer(string serverId, string serverName, string flavor, string diskConfig = null, string region = null, CloudIdentity identity = null);
        bool ConfirmServerResize(string serverId, string region = null, CloudIdentity identity = null);
        bool RevertServerResize(string serverId, string region = null, CloudIdentity identity = null);
        string RescueServer(string serverId, string region = null, CloudIdentity identity = null);
        ServerDetails UnRescueServer(string serverId, string region = null, CloudIdentity identity = null);
        bool CreateImage(string serverId, string imageName, Metadata metadata = null, string region = null, CloudIdentity identity = null);

        // Volume Attachment Actions

        // Flavors
        IEnumerable<Flavor> ListFlavors(int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null, CloudIdentity identity = null);
        IEnumerable<FlavorDetails> ListFlavorsWithDetails(int minDiskInGB = 0, int minRamInMB = 0, string markerId = null, int limit = 0, string region = null, CloudIdentity identity = null);
        FlavorDetails GetFlavor(string id, string region = null, CloudIdentity identity = null);
            
        // Images
        IEnumerable<ServerImage> ListImages(string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null, CloudIdentity identity = null);
        IEnumerable<ServerImageDetails> ListImagesWithDetails(string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, string imageType = null, string region = null, CloudIdentity identity = null);
        ServerImageDetails GetImage(string imageId, string region = null, CloudIdentity identity = null);
        bool DeleteImage(string imageId, string region = null, CloudIdentity identity = null);

        // Server metadata
        Metadata ListServerMetadata(string cloudServerId, string region = null, CloudIdentity identity = null);
        bool SetServerMetadata(string cloudServerId, Metadata metadata, string region = null, CloudIdentity identity = null);
        bool UpdateServerMetadata(string cloudServerId, Metadata metadata, string region = null, CloudIdentity identity = null);
        string GetServerMetadataItem(string cloudServerId, string key, string region = null, CloudIdentity identity = null);
        bool SetServerMetadataItem(string cloudServerId, string key, string value, string region = null, CloudIdentity identity = null);
        bool DeleteServerMetadataItem(string cloudServerId, string key, string region = null, CloudIdentity identity = null);
    
        // Image metadata
        Metadata ListImageMetadata(string cloudServerId, string region = null, CloudIdentity identity = null);
        bool SetImageMetadata(string cloudServerId, Metadata metadata, string region = null, CloudIdentity identity = null);
        bool UpdateImageMetadata(string cloudServerId, Metadata metadata, string region = null, CloudIdentity identity = null);
        string GetImageMetadataItem(string cloudServerId, string key, string region = null, CloudIdentity identity = null);
        bool SetImageMetadataItem(string cloudServerId, string key, string value, string region = null, CloudIdentity identity = null);
        bool DeleteImageMetadataItem(string cloudServerId, string key, string region = null, CloudIdentity identity = null);
        
        ServerDetails WaitForServerState(string cloudServerId, string expectedState, string[] errorStates, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400, CloudIdentity identity = null);
        ServerDetails WaitForServerActive(string cloudServerId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400, CloudIdentity identity = null);
        void WaitForServerDeleted(string cloudServerId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400, CloudIdentity identity = null);
        ServerImageDetails WaitForImageState(string imageId, string expectedState, string[] errorStates, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400, CloudIdentity identity = null);
        ServerImageDetails WaitForImageActive(string imageId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400, CloudIdentity identity = null);
        void WaitForImageDeleted(string imageId, string region = null, int refreshCount = 600, int refreshDelayInMS = 2400, CloudIdentity identity = null);
    }
}
    