namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateCloudBlockStorageSnapshotRequest
    {
        [JsonProperty("snapshot")]
        public CreateCloudBlockStorageSnapshotDetails CreateCloudBlockStorageSnapshotDetails { get; set; } 
    }
}
