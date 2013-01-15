using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    public class UpdateMetadataItemRequest
    {
        [DataMember(Name = "meta")]
        public Metadata Metadata { get; set; }
    }
}