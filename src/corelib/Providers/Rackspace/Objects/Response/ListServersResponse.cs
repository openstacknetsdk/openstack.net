namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListServersResponse
    {
        [JsonProperty("servers")]
        public Server[] Servers { get; private set; }

        [JsonProperty("servers_links")]
        public Link[] Links { get; private set; }
    }
}
