namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListSnapshotResponse
    {
        [JsonProperty("snapshots")]
        public Snapshot[] Snapshots { get; set; }
    }
}
