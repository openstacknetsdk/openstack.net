using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Personality
    {
        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "contents")]
        public string Content { get; set; }
    }
}
