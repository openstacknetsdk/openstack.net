namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateVirtualInterfaceRequest
    {
        [JsonProperty("virtual_interface")]
        public CreateVirtualInterface VirtualInterface { get; set; }

        public CreateVirtualInterfaceRequest(string networkId)
        {
            VirtualInterface = new CreateVirtualInterface {NetworkId = networkId};
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class CreateVirtualInterface
    {
        [JsonProperty("network_id")]
        public string NetworkId { get; set; }
    }
}
