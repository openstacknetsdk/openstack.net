using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class AddRoleRequest
    {
        [DataMember(Name = "role")]
        public Role Role { get; set; } 

    }
}
