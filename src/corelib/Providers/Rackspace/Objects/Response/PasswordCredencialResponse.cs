namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Providers.Rackspace.Objects.Request;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class PasswordCredencialResponse
    {
        [JsonProperty("passwordCredentials")]
        public PasswordCredencial PasswordCredencial { get; set; }
    }
}
