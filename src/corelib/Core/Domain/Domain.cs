using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Domain
    {
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        [DataMember(Name = "accountId")]
        public int? AccountId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "ttl")]
        public int Ttl { get; set; }

        [DataMember(Name = "emailAddress")]
        public string EmailAddress { get; set; }

        [DataMember(Name = "updated")]
        public DateTime? Updated { get; internal set; }

        [DataMember(Name = "created")]
        public DateTime? Created { get; internal set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "nameservers")]
        public Nameserver[] Nameservers { get; set; }

        [DataMember(Name = "recordsList")]
        public RecordsList RecordsList { get; set; }

        [DataMember(Name = "subdomains")]
        public Subdomains Subdomains { get; set; }
    }
}
