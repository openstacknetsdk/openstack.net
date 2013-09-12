namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class RescueServerResponse
    {
        [JsonProperty("adminPass")]
        public string AdminPassword { get; private set; }
    }
}
