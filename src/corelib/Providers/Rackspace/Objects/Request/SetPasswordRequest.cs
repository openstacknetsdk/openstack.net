namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class SetPasswordRequest
    {
        [JsonProperty("passwordCredentials")]
        public PasswordCredencial PasswordCredencial { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class PasswordCredencial
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
