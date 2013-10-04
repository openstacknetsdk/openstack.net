using System.Runtime.Serialization;

namespace net.openstack.Core.Domain.Rackspace
{
    [DataContract]
    public class RecordsList
    {
        [DataMember(Name = "totalEntries")]
        public int? TotalEntries { get; set; }

        [DataMember(Name = "records")]
        public Record[] Records { get; set; }
    }
}
