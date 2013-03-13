using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ContainerObject
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Hash { get; set; }

        [DataMember]
        public long Bytes { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "last_modified")]
        public DateTime LastModified { get; set; }
    }
}
