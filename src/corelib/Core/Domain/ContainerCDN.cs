namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ContainerCDN
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cdn_streaming_uri")]
        public string CDNStreamingUri { get; set; }

        [JsonProperty("cdn_ssl_uri")]
        public string CDNSslUri { get; set; }

        [JsonProperty("cdn_enabled")]
        public bool CDNEnabled { get; set; }

        [JsonProperty("ttl")]
        public long Ttl { get; set; }

        [JsonProperty("log_retention")]
        public bool LogRetention { get; set; }

        [JsonProperty("cdn_uri")]
        public string CDNUri { get; set; }

        [JsonProperty("cdn_ios_uri")]
        public string CDNIosUri { get; set; }

    }
}
