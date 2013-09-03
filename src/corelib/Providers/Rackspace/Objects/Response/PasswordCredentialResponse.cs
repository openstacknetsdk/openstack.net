namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Providers.Rackspace.Objects.Request;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class PasswordCredentialResponse
    {
        [JsonProperty("passwordCredentials")]
        public PasswordCredential PasswordCredential { get; set; }
    }
}
