using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    [DataContract]
    public class RolesResponse
    {
        [DataMember]
        public Role[] Roles { get; set; }

        [DataMember(Name="roles_links")]
        public string[] RoleLinks { get; set; }
    }
}