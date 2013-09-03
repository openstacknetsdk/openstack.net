namespace net.openstack.Providers.Rackspace.Objects.Response
{
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ListAddressesByNetworkResponse
    {
        [JsonProperty]
        public KeyValuePair<string, AddressDetails[]> Network { get; set; } 
    }
}
