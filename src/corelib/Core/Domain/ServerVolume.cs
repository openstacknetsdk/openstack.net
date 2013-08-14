namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServerVolume
    {
        [JsonProperty("device")]
        public string Device { get; private set; }

        [JsonProperty("serverId")]
        public string ServerId { get; private set; }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("volumeId")]
        public string VolumeId { get; private set; }
    }
}
