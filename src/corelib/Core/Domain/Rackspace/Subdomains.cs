using System.Runtime.Serialization;

namespace net.openstack.Core.Domain.Rackspace
{
    [DataContract]
    public class Subdomains
    {
        [DataMember(Name = "totalEntries")]
        public int? TotalEntries { get; set; }

        [DataMember(Name = "domains")]
        public Rackspace.Domain[] Domains { get; set; }
    }
}
