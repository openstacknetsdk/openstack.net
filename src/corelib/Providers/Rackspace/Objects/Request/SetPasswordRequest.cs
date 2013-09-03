namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class SetPasswordRequest
    {
        [JsonProperty("passwordCredentials")]
        public PasswordCredential PasswordCredential { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class PasswordCredential
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
