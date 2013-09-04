namespace net.openstack.Core.Domain
{
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;

    [JsonDictionary]
    public class ServerAddresses : Dictionary<string, IPAddressList>
    {
        [JsonIgnore]
        public IList<IPAddress> Private { get { return ContainsKey("private") ? this["private"] : null; } }

        [JsonIgnore]
        public IList<IPAddress> Public { get { return ContainsKey("public") ? this["public"] : null; } }
    }
}