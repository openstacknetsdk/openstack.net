namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("rel")]
        public string Rel { get; set; }
    }
}