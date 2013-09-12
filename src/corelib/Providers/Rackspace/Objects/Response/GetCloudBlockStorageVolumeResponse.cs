namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class GetCloudBlockStorageVolumeResponse
    {
        [JsonProperty("volume")]
        public Volume Volume { get; private set; }
    }
}
