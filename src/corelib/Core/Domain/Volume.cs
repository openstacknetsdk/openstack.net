using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Volume
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "display_description")]
        public string DisplayDescription { get; set; }

        [DataMember]
        public int Size { get; set; }

        [DataMember(Name = "volume_type")]
        public string VolumeType { get; set; }

        [DataMember(Name = "snapshot_id")]
        public string SnapshotId { get; set; }

        [DataMember]
        public Dictionary<string, string>[] Attachments { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember(Name = "availability_zone")]
        public string AvailabilityZone { get; set; }

        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
