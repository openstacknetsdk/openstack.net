namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UnrescueServerRequest
    {
        [JsonProperty("unrescue")]
        public string Details { get; set; }
    }
}