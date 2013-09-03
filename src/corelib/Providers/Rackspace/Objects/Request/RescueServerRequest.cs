namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class RescueServerRequest
    {
        [JsonProperty("rescue")]
        public string Details { get; set; }
    }
}
