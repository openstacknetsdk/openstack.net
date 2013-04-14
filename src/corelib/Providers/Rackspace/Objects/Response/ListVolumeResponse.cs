using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListVolumeResponse
    {
        [DataMember(Name = "volumes")]
        public Volume[] Volumes { get; set; }
    }
}
