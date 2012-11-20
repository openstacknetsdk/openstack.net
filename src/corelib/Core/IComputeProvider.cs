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
        NewServer CreateServer(CloudIdentity identity, string cloudServerName, string friendlyName, string imageName, string flavor, string region = null);
        ServerDetails GetDetails(CloudIdentity identity, string cloudServerId, string region = null);
        bool UpdateServer(CloudIdentity identity, string cloudServerId, string name = null, string ipV4Address = null, string ipV6Address = null, string region = null);
        bool DeleteServer(CloudIdentity identity, string cloudServerId, string region = null);
        
        // Server Addresses
        ServerAddresses ListAddresses(CloudIdentity identity, string serverId, string region = null);
        Network ListAddressesByNetwork(CloudIdentity identity, string serverId, string network, string region = null);
        
        // Server Actions

        // Volume Attachment Actions

        // Flavors
        IEnumerable<Flavor> ListFlavors(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerID = null, int limit = 0, string region = null);
        IEnumerable<FlavorDetails> ListFlavorsWithDetails(CloudIdentity identity, int minDiskInGB = 0, int minRamInMB = 0, string markerID = null, int limit = 0, string region = null);
        FlavorDetails GetFlavor(CloudIdentity identity, string id, string region = null);
            
        // Images
        IEnumerable<ServerImage> ListImages(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, ImageType imageType = ImageType.NONE, string region = null);
        IEnumerable<ServerImageDetails> ListImagesWithDetails(CloudIdentity identity, string serverId = null, string imageName = null, string imageStatus = null, DateTime changesSince = default(DateTime), string markerId = null, int limit = 0, ImageType imageType = ImageType.NONE, string region = null);
        ServerImageDetails GetImage(CloudIdentity identity, string imageId, string region = null);
        bool DeleteImage(CloudIdentity identity, string imageId, string region = null);

        // Metadata
        Metadata ListMetadata(CloudIdentity identity, string cloudServerId, string region = null);
    }
}
    