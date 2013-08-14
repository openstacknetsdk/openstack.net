namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateCloudBlockStorageVolumeRequest
    {
        [JsonProperty("volume")]
        public CreateCloudBlockStorageVolumeDetails CreateCloudBlockStorageVolumeDetails { get; set; } 
    }
}
