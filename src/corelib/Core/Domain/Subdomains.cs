using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Subdomains
    {
        [DataMember(Name = "totalEntries")]
        public int? TotalEntries { get; set; }

        [DataMember(Name = "domains")]
        public Domain[] Domains { get; set; }
    }
}
