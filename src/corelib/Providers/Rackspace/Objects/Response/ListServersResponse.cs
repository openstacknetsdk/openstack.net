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
        public ServerLink[] Links { get; private set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerLink
    {
        [JsonProperty("href")]
        public string Link { get; private set; }

        [JsonProperty("rel")]
        public string Type { get; private set; }
    }
}
