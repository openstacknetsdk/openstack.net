using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class ServerVolume
    {
        [DataMember(Name ="device")]
        public string Device { get; set; }

        [DataMember(Name = "serverId")]
        public string ServerId { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "volumeId")]
        public string VolumeId { get; set; }
    }
}
