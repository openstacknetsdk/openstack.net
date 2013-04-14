using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface ICloudBlockStorageProvider
    {
        bool CreateVolume(int size, string display_description = null, string display_name = null, string snapshot_id = null, string volume_type = null, string region = null, CloudIdentity identity = null);
        IEnumerable<Volume> ListVolumes(string region = null, CloudIdentity identity = null);
        Volume ShowVolume(string volume_id, string region = null, CloudIdentity identity = null);
        bool DeleteVolume(string volume_id, string region = null, CloudIdentity identity = null);

        IEnumerable<VolumeType> ListVolumeTypes(string region = null, CloudIdentity identity = null);
        VolumeType DescribeVolumeType(int volume_type_id, string region = null, CloudIdentity identity = null);

        bool CreateSnapshot(string volume_id, bool force = false, string display_name = "None", string display_description = null, string region = null, CloudIdentity identity = null);
        IEnumerable<Snapshot> ListSnapshots(string region = null, CloudIdentity identity = null);
        Snapshot ShowSnapshot(string snapshot_id, string region = null, CloudIdentity identity = null);
        bool DeleteSnapshot(string snapshot_id, string region = null, CloudIdentity identity = null);


    }
}
