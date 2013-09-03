namespace net.openstack.Core.Domain
{
    using System;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ContainerObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hash")]
        public Guid Hash { get; set; }

        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }
    }
}
