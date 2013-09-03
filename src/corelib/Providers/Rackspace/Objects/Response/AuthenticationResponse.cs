namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class AuthenticationResponse
    {
        [JsonProperty("access")]
        public UserAccess UserAccess { get; private set; }
    }
}
