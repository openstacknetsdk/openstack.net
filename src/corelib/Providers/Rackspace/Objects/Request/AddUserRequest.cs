using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class AddUserRequest
    {
        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}
