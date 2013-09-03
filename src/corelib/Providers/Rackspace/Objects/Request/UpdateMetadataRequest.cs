namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdateMetadataRequest
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}
