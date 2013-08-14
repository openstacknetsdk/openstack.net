namespace net.openstack.Core.Domain
{
    using System.Net;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualInterfaceAddress
    {
        [JsonProperty("address")]
        private string _address;

        public IPAddress Address
        {
            get
            {
                if (_address == null)
                    return null;

                return IPAddress.Parse(_address);
            }

            private set
            {
                if (value == null)
                {
                    _address = null;
                    return;
                }

                _address = value.ToString();
            }
        }

        [JsonProperty("network_id")]
        public string NetworkId { get; private set; }

        [JsonProperty("network_label")]
        public string NetworkLabel { get; private set; }
    }
}