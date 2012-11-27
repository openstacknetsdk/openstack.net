using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ServerRebootDetails
    {
        [DataMember(Name = "type")]
        public RebootType Type { get; set; }
    }
}