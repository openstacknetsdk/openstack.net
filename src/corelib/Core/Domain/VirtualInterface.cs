namespace net.openstack.Core.Domain
{
    using System.Net.NetworkInformation;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterface
    {
        [JsonProperty("mac_address")]
        private string _macAddress;

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("ip_addresses")]
        public VirtualInterfaceAddress[] Addresses { get; private set; }

        public PhysicalAddress MACAddress
        {
            get
            {
                if (_macAddress == null)
                    return null;

                return PhysicalAddress.Parse(_macAddress);
            }
        }
    }
}
