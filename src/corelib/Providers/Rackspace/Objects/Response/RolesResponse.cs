namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class RolesResponse
    {
        [JsonProperty]
        public Role[] Roles { get; set; }

        [JsonProperty("roles_links")]
        public string[] RoleLinks { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class RoleResponse
    {
        [JsonProperty]
        public Role Role { get; set; }
    }
}
