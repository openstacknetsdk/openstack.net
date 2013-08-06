using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Nameserver
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
