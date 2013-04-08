using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class GetCloudBlockStorageVolumeTypeResponse
    {
        [DataMember(Name = "volume_type")]
        public VolumeType VolumeType { get; set; }
    }
}
