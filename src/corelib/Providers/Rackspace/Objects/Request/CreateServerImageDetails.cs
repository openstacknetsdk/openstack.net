namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateServerImageDetails
    {
        [JsonProperty("name")]
        public string ImageName { get; set; }

        [JsonProperty("metadata", DefaultValueHandling = DefaultValueHandling.Include)]
        public Metadata Metadata { get; set; }
    }
}
