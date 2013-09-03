namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class UpdateServerRequest
    {
        [JsonProperty("server")]
        public ServerUpdateDetails Server { get; set; }

        public UpdateServerRequest()
        {
            Server = new ServerUpdateDetails();
        }

        public UpdateServerRequest(string name, string accessIPv4, string accessIPv6)
        {
            Server = new ServerUpdateDetails{Name = name, AccessIpV4 = accessIPv4, AccessIPv6 = accessIPv6};
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class ServerUpdateDetails
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; set; }

        [JsonProperty("accessIPv4", DefaultValueHandling = DefaultValueHandling.Include)]
        public string AccessIpV4 { get; set; }

        [JsonProperty("accessIPv6", DefaultValueHandling = DefaultValueHandling.Include)]
        public string AccessIPv6 { get; set; }
    }
}
