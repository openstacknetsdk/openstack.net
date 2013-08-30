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

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        [JsonProperty("display_description")]
        public string DisplayDescription { get; private set; }

        [JsonProperty("size")]
        public int Size { get; private set; }

        [JsonProperty("volume_type")]
        public string VolumeType { get; private set; }

        [JsonProperty("snapshot_id")]
        public string SnapshotId { get; private set; }

        [JsonProperty("attachments")]
        public Dictionary<string, string>[] Attachments { get; private set; }

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
        public string AvailabilityZone { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; private set; }
    }
}
