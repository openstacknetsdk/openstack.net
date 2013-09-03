namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Endpoint
    {
        [JsonProperty("publicURL")]
        public string PublicURL { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("versionId")]
        public string VersionId { get; set; }

        [JsonProperty("versionInfo")]
        public string VersionInfo { get; set; }

        [JsonProperty("versionList")]
        public string VersionList { get; set; }

        [JsonProperty("internalURL")]
        public string InternalURL { get; set; }
    }
}