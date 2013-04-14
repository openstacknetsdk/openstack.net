using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListVolumeTypeResponse
    {
        [DataMember(Name = "volume_types")]
        public VolumeType[] VolumeTypes { get; set; }
    }
}
