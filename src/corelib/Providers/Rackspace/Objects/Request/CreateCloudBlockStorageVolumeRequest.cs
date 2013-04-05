using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateCloudBlockStorageVolumeRequest
    {
        [DataMember(Name = "volume")]
        public CreateCloudBlockStorageVolumeDetails CreateCloudBlockStorageVolumeDetails { get; set; } 
    }
}
