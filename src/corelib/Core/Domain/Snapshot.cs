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
        public string Id { get; private set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; private set; }

        [JsonProperty("display_description")]
        public string DisplayDescription { get; private set; }

        [JsonProperty("volume_id")]
        public string VolumeId { get; private set; }

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
        public string Size { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; private set; }
    }
}
