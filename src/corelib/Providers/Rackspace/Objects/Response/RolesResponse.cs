namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    /// <summary>
    /// This models the JSON response used for the List Roles and List User Global Roles requests.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/openstack-extensions/auth/OS-KSADM-admin-devguide/content/GET_listRoles_v2.0_OS-KSADM_roles_Admin_API_Service_Developer_Operations-d1e1357.html">List Roles (Rackspace OS-KSADM Extension - API v2.0)</seealso>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/GET_listUserGlobalRoles_v2.0_users__user_id__roles_User_Operations.html">List User Global Roles (OpenStack Identity Service API v2.0 Reference)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    internal class RolesResponse
    {
        /// <summary>
        /// Gets a collection of roles.
        /// </summary>
        [JsonProperty("roles")]
        public Role[] Roles { get; private set; }

        /// <summary>
        /// Gets a collection of links related to <see cref="Roles"/>.
        /// </summary>
        [JsonProperty("roles_links")]
        public string[] RoleLinks { get; private set; }
    }

    /// <summary>
    /// This models the JSON response used for the Add Role and Get Role by Name requests.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_addRole_v2.0_OS-KSADM_roles_Role_Operations_OS-KSADM.html">Add Role (OpenStack Identity Service API v2.0 Reference)</seealso>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/GET_getRoleByName_v2.0_OS-KSADM_roles_Role_Operations_OS-KSADM.html">Get Role by Name (OpenStack Identity Service API v2.0 Reference)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    internal class RoleResponse
    {
        /// <summary>
        /// Gets information about the role.
        /// </summary>
        [JsonProperty("role")]
        public Role Role { get; private set; }
    }
}
