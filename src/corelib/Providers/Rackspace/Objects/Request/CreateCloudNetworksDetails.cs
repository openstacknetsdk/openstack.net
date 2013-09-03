namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class CreateCloudNetworksDetails
    {
        [JsonProperty("cidr")]
        public string Cidr { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
