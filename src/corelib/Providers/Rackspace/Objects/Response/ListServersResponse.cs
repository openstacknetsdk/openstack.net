namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListServersResponse
    {
        [JsonProperty("servers")]
        public Server[] Servers { get; set; }

        [JsonProperty("servers_links")]
        public ServerLink[] Links { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerLink
    {
        [JsonProperty("href")]
        public string Link { get; set; }

        [JsonProperty("rel")]
        public string Type { get; set; }
    }
}