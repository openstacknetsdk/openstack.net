namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class MetaDataResponse
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; private set; }
    }
}
