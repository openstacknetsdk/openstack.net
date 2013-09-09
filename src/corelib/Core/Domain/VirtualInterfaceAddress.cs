namespace net.openstack.Core.Domain
{
    using System.Net;
    using net.openstack.Core.Domain.Converters;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterfaceAddress
    {
        [JsonProperty("address")]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        public IPAddress Address
        {
            get;
            private set;
        }

        [JsonProperty("network_id")]
        public string NetworkId { get; private set; }

        [JsonProperty("network_label")]
        public string NetworkLabel { get; private set; }
    }
}