namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CloudNetworkResponse
    {
        [JsonProperty("network")]
        public CloudNetwork Network { get; set; }
    }
}
