namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateCloudBlockStorageSnapshotDetails
    {
        [JsonProperty("volume_id")]
        public string VolumeId { get; set; }

        [JsonProperty]
        public bool Force { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        
        [JsonProperty("display_description")]
        public string DisplayDescription { get; set; }
    }
}
