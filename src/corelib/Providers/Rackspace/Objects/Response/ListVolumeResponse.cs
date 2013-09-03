namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListVolumeResponse
    {
        [JsonProperty("volumes")]
        public Volume[] Volumes { get; set; }
    }
}
