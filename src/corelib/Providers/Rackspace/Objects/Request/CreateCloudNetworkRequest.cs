namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateCloudNetworkRequest
    {
        [JsonProperty("network")]
        public CreateCloudNetworksDetails Details { get; set; }
    }
}
