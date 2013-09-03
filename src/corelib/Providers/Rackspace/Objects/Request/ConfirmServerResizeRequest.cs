namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ConfirmServerResizeRequest
    {
        [JsonProperty("confirmResize")]
        public string Details { get; set; }

        public ConfirmServerResizeRequest()
        {
            Details = string.Empty;
        }
    }
}