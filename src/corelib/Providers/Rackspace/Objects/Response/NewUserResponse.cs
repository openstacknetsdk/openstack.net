namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class NewUserResponse
    {
        [JsonProperty("user")]
        public NewUser NewUser { get; private set; }
    }
}