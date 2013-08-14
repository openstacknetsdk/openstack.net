namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerResizeRequest
    {
        [JsonProperty("resize")]
        public ServerResizeDetails Details { get; set; }
    }
}