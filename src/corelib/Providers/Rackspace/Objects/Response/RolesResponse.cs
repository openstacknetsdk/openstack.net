namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class RolesResponse
    {
        [JsonProperty("roles")]
        public Role[] Roles { get; private set; }

        [JsonProperty("roles_links")]
        public string[] RoleLinks { get; private set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class RoleResponse
    {
        [JsonProperty("role")]
        public Role Role { get; private set; }
    }
}
