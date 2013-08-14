namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class AttachServerVolumeRequest
    {
        [JsonProperty("volumeAttachment")]
        public AttachServerVolumeData ServerVolumeData { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class AttachServerVolumeData
    {
        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("volumeId")]
        public string VolumeId { get; set; }
    }
}
