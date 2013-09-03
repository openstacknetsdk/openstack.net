namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class RevertServerResizeRequest
    {
        [JsonProperty("revertResize")]
        public string Details { get; set; }

        public RevertServerResizeRequest()
        {
            Details = string.Empty;
        }
    }
}
