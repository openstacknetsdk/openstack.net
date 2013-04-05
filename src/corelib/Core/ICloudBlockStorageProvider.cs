using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface ICloudBlockStorageProvider
    {
        bool CreateVolume(int size, string display_description = null, string display_name = null, string snapshot_id = null, string volume_type = null, string region = null, CloudIdentity identity = null);
    }
}
