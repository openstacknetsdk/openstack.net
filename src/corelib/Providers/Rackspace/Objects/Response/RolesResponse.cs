using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class RolesResponse
    {
        [DataMember]
        public Role[] Roles { get; set; }

        [DataMember(Name="roles_links")]
        public string[] RoleLinks { get; set; }
    }

    [DataContract]
    internal class RoleResponse
    {
        [DataMember]
        public Role Role { get; set; }
    }
}