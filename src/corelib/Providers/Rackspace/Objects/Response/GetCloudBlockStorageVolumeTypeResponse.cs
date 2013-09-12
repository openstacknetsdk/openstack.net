namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class GetCloudBlockStorageVolumeTypeResponse
    {
        [JsonProperty("volume_type")]
        public VolumeType VolumeType { get; private set; }
    }
}
