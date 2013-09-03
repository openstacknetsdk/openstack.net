namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class AddUserRequest
    {
        [JsonProperty("user")]
        public NewUser User { get; set; }
    }
}
