namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerRebuildDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageRef")]
        public string ImageName { get; set; }

        [JsonProperty("flavorRef")]
        public string Flavor { get; set; }

        [JsonProperty("OS-DCF:diskConfig", DefaultValueHandling = DefaultValueHandling.Include)]
        public string DiskConfig { get; set; }

        [JsonProperty("adminPass")]
        public string AdminPassword { get; set; }

        [JsonProperty("metadata", DefaultValueHandling = DefaultValueHandling.Include)]
        public Dictionary<string, string> Metadata { get; set; }

        [JsonProperty("personality", DefaultValueHandling = DefaultValueHandling.Include)]
        public Personality Personality { get; set; }

        [JsonProperty("accessIPv4", DefaultValueHandling = DefaultValueHandling.Include)]
        public string AccessIPv4 { get; set; }

        [JsonProperty("accessIPv6", DefaultValueHandling = DefaultValueHandling.Include)]
        public string AccessIPv6 { get; set; }
    }
}