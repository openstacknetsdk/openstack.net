namespace net.openstack.Core.Domain
{
    using System.Net;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Network
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("ip", ItemConverterType = typeof(IPAddressConverter))]
        public IPAddress[] Addresses { get; private set; }

        public Network(string id, IPAddress[] addresses)
        {
            Id = id;
            Addresses = addresses;
        }
    }
}
