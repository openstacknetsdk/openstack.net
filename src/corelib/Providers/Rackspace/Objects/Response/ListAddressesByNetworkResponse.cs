namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListAddressesByNetworkResponse
    {
        [JsonProperty]
        public KeyValuePair<string, IPAddress[]> Network { get; set; } 
    }
}
