using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Container
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public long Bytes { get; set; }
    }
}
