namespace net.openstack.Core.Domain
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [JsonDictionary]
    public class ServerAddresses : Dictionary<string, AddressDetails[]>
    {
        [JsonIgnore]
        public AddressDetails[] Private { get { return ContainsKey("private") ? this["private"] : null; } }

        [JsonIgnore]
        public AddressDetails[] Public { get { return ContainsKey("public") ? this["public"] : null; } }
    }
}