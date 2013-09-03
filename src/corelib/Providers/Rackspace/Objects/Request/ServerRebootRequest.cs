namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerRebootRequest
    {
        [JsonProperty("reboot")]
        public ServerRebootDetails Details { get; set; }
    }
}
