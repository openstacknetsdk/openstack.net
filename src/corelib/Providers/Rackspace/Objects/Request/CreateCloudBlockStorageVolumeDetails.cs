namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateCloudBlockStorageVolumeDetails
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("display_description")]
        public string DisplayDescription { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("snapshot_id")]
        public string SnapshotId { get; set; }

        [JsonProperty("volume_type")]
        public string VolumeType { get; set; }
    }
}
