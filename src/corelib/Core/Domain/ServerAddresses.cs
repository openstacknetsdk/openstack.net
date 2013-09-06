namespace net.openstack.Core.Domain
{
    using System.Collections.Generic;
    using System.Net;
    using Newtonsoft.Json;

    [JsonDictionary]
    public class ServerAddresses : Dictionary<string, IPAddressList>
    {
        [JsonIgnore]
        public IList<IPAddress> Private
        {
            get
            {
                IPAddressList result;
                if (!TryGetValue("private", out result))
                    return null;

                return result;
            }
        }

        [JsonIgnore]
        public IList<IPAddress> Public
        {
            get
            {
                IPAddressList result;
                if (!TryGetValue("public", out result))
                    return null;

                return result;
            }
        }
    }
}
