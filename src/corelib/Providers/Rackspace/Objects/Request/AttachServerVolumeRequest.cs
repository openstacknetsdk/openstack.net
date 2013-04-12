using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class AttachServerVolumeRequest
    {
        [DataMember(Name = "volumeAttachment")]
        public AttachServerVolumeData ServerVolumeData { get; set; }
    }
}
