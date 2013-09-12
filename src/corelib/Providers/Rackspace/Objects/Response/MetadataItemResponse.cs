namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class MetadataItemResponse
    {
        [JsonProperty("meta")]
        public Metadata Metadata { get; private set; }
    }
}
