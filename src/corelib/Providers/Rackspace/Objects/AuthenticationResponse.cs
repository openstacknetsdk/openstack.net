using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    [DataContract]
    public class AuthenticationResponse
    {
        [DataMember(Name = "access")]
        public UserAccess UserAccess { get; set; }
    }
}
