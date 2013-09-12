namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class GetImageDetailsResponse
    {
        [JsonProperty("image")]
        public ServerImage Image { get; private set; }
    }
}
