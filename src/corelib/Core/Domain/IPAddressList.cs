namespace net.openstack.Core.Domain
{
    using System.Collections.Generic;
    using System.Net;
    using net.openstack.Core.Domain.Converters;
    using Newtonsoft.Json;

    [JsonArray(ItemConverterType = typeof(IPAddressDetailsConverter))]
    public class IPAddressList : List<IPAddress>
    {
        public IPAddressList()
        {
        }

        public IPAddressList(int capacity)
            : base(capacity)
        {
        }

        public IPAddressList(IEnumerable<IPAddress> collection)
            : base(collection)
        {
        }
    }
}
