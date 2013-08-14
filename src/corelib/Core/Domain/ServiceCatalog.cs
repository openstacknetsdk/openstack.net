namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceCatalog
    {
        [JsonProperty("endpoints")]
        public Endpoint[] Endpoints { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; }
    }
}