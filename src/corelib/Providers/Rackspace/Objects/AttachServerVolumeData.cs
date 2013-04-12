using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Providers.Rackspace.Objects
{
    [DataContract]
    internal class AttachServerVolumeData
    {
        [DataMember(Name = "device")]
        public string Device { get; set; }

        [DataMember(Name = "volumeId")]
        public string VolumeId { get; set; }
    }
}
