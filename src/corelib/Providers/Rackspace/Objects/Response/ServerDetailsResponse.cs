namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerDetailsResponse
    {
        [JsonProperty("server")]
        public Server Server { get; private set; }
    }
}
