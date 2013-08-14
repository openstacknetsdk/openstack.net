namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Flavor
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}