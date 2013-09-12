namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListVolumeTypeResponse
    {
        [JsonProperty("volume_types")]
        public VolumeType[] VolumeTypes { get; private set; }
    }
}
