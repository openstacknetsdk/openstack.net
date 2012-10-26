using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IComputeProvider
    {
        MetaData GetMetaData(string apiServerId, CloudIdentity identity);
        ServerDetails GetDetails(string apiServerId, CloudIdentity identity);
        NewServer CreateServer(string cloudServerName, string friendlyName, string imageName, string flavor, CloudIdentity identity);
        bool DeleteServer(string cloudServerId, CloudIdentity identity);
    }
}
