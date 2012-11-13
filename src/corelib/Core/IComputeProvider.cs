using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IComputeProvider
    {
        IEnumerable<Server> ListServers(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null);
        IEnumerable<ServerDetails> ListServersWithDetails(CloudIdentity identity, string imageId = null, string flavorId = null, string name = null, string status = null, string markerId = null, int? limit = null, DateTime? changesSince = null);
        Metadata ListMetadata(string cloudServerId, CloudIdentity identity);
        ServerDetails GetDetails(string cloudServerId, CloudIdentity identity);
        NewServer CreateServer(string cloudServerName, string friendlyName, string imageName, string flavor, CloudIdentity identity);
        bool UpdateServer(string cloudServerId, CloudIdentity identity, string name = null, string ipV4Address = null, string ipV6Address = null);
        bool DeleteServer(string cloudServerId, CloudIdentity identity);
    }
}
    