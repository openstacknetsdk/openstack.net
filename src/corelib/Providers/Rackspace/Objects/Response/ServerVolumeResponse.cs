namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServerVolumeListResponse
    {
        [JsonProperty("volumeAttachments")]
        public IEnumerable<ServerVolume> ServerVolumes { get; set; }
    }
}
