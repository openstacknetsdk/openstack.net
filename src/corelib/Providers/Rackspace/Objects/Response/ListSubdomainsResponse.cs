using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListSubdomainsResponse
    {
        [DataMember(Name = "subdomains")]
        public Core.Domain.Domain[] Domains { get; set; }

        [DataMember(Name = "totalEntries")]
        public int TotalEntries { get; set; }
    }
}
