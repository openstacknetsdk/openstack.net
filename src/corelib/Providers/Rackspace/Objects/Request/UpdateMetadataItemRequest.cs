namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdateMetadataItemRequest
    {
        [JsonProperty("meta")]
        public Metadata Metadata { get; set; }
    }
}