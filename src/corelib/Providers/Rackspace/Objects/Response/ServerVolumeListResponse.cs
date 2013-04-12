using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ServerVolumeResponse
    {
        [DataMember(Name = "volumeAttachment")]
        public ServerVolume ServerVolume { get; set; }
    }
}