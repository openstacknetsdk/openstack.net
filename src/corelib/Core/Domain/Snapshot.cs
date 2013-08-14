namespace net.openstack.Core.Domain
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Snapshot
    {
        [JsonProperty("status")]
        private string _status;

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("display_description")]
        public string DisplayDescription { get; set; }

        [JsonProperty("volume_id")]
        public string VolumeId { get; set; }

        public SnapshotState Status
        {
            get
            {
                if (string.IsNullOrEmpty(_status))
                    return null;

                return SnapshotState.FromName(_status);
            }
        }

        [JsonProperty]
        public string Size { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
