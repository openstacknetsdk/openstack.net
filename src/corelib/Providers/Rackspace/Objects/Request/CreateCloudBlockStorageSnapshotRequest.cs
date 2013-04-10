using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateCloudBlockStorageSnapshotRequest
    {
        [DataMember(Name = "snapshot")]
        public CreateCloudBlockStorageSnapshotDetails CreateCloudBlockStorageSnapshotDetails { get; set; } 
    }
}