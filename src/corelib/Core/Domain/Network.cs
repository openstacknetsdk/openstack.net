namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Network
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("ip")]
        public AddressDetails[] Addresses { get; private set; }

        public Network(string id, AddressDetails[] addresses)
        {
            Id = id;
            Addresses = addresses;
        }
    }
}