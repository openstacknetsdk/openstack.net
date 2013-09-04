namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserDetails
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("roles")]
        public Role[] Roles { get; private set; }

        [JsonProperty("RAX-AUTH:defaultRegion")]
        public string DefaultRegion { get; private set; }
    }
}
