namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerRebuildRequest
    {   
        [JsonProperty("rebuild")]
        public ServerRebuildDetails Details { get; set; }
    }
}
