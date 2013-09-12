namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UsersResponse
    {
        [JsonProperty("users")]
        public User[] Users { get; private set; }
    }
}
