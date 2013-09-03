namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class User
    {
        [JsonProperty("RAX-AUTH:defaultRegion")]
        public string DefaultRegion { get; set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}