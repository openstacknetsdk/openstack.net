namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UserImpersonationResponse
    {
        [JsonProperty("userAccess")]
        public UserAccess UserAccess { get; private set; }
    }
}
