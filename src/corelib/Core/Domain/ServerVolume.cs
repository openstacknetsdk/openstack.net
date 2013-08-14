namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServerVolume
    {
        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("serverId")]
        public string ServerId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("volumeId")]
        public string VolumeId { get; set; }
    }
}
