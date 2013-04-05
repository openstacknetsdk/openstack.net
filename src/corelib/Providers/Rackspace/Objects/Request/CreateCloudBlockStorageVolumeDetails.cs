using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    public class CreateCloudBlockStorageVolumeDetails
    {
        [DataMember(Name = "size")]
        public int Size { get; set; }

        [DataMember(Name = "display_description")]
        public string DisplayDescription { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "snapshot_id")]
        public string SnapshotId { get; set; }

        [DataMember(Name = "volume_type")]
        public string VolumeType { get; set; }
    }
}
