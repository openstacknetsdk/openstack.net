namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListImagesResponse
    {
        [JsonProperty("images")]
        public SimpleServerImage[] Images { get; private set; }
    }
}
