namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UserCredentialResponse
    {
        [JsonProperty("RAX-KSKEY:apiKeyCredentials")]
        public UserCredential UserCredential { get; private set; }
    }
}
