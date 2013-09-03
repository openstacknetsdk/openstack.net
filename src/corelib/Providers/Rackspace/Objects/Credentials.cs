namespace net.openstack.Providers.Rackspace.Objects
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class Credentials
    {
        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Username { get; set; }

        [JsonProperty("password", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Password { get; set; }

        [JsonProperty("apiKey", DefaultValueHandling = DefaultValueHandling.Include)]
        public string APIKey { get; set; }
    }
}
