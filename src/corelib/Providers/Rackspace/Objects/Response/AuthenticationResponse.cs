using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class AuthenticationResponse
    {
        [DataMember(Name = "access")]
        public UserAccess UserAccess { get; set; }
    }
}
