using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IComputeProvider
    {
        IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null);
        IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null, string region = null);
        Metadata ListMetadata(CloudIdentity identity, string cloudServerId, string region = null);
        ServerDetails GetDetails(CloudIdentity identity, string cloudServerId, string region = null);
        NewServer CreateServer(CloudIdentity identity, string cloudServerName, string friendlyName, string imageName, string flavor, string region = null);
        bool UpdateServer(CloudIdentity identity, string cloudServerId, string name = null, string ipV4Address = null, string ipV6Address = null, string region = null);
        bool DeleteServer(CloudIdentity identity, string cloudServerId, string region = null);
    }
}
    