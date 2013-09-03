namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateServerImageRequest
    {
        [JsonProperty("createImage")]
        public CreateServerImageDetails Details { get; set; }
    }
}
