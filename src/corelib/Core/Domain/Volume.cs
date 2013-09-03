namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Volume
    {
        [JsonProperty("status")]
        private string _status;

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("display_description")]
        public string DisplayDescription { get; set; }

        [JsonProperty]
        public int Size { get; set; }

        [JsonProperty("volume_type")]
        public string VolumeType { get; set; }

        [JsonProperty("snapshot_id")]
        public string SnapshotId { get; set; }

        [JsonProperty]
        public Dictionary<string, string>[] Attachments { get; set; }

        public VolumeState Status
        {
            get
            {
                if (string.IsNullOrEmpty(_status))
                    return null;

                return VolumeState.FromName(_status);
            }
        }

        [JsonProperty("availability_zone")]
        public string AvailabilityZone { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
