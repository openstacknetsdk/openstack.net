namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerVolumeResponse
    {
        [JsonProperty("volumeAttachment")]
        public ServerVolume ServerVolume { get; private set; }
    }
}