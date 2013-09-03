namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerResizeDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("flavorRef")]
        public string Flavor { get; set; }

        [JsonProperty("OS-DCF:diskConfig", DefaultValueHandling = DefaultValueHandling.Include)]
        public string DiskConfig { get; set; }
    }
}
