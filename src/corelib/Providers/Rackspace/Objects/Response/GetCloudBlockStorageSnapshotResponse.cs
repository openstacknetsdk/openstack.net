namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class GetCloudBlockStorageSnapshotResponse
    {
        [JsonProperty("snapshot")]
        public Snapshot Snapshot { get; private set; }
    }
}
