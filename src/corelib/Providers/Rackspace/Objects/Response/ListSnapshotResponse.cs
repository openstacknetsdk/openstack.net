using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListSnapshotResponse
    {
        [DataMember(Name = "snapshots")]
        public Snapshot[] Snapshots { get; set; }
    }
}
