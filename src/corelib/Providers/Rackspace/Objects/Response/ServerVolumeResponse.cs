using System.Collections.Generic;
using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    public class ServerVolumeListResponse
    {
        [DataMember(Name = "volumeAttachments")]
        public IEnumerable<ServerVolume> ServerVolumes { get; set; }
    }
}
