namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListImagesDetailsResponse
    {
        [JsonProperty("images")]
        public ServerImage[] Images { get; private set; }
    }
}
