using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateVirtualInterfaceRequest
    {
        [DataMember(Name = "virtual_interface")]
        public CreateVirtualInterface VirtualInterface { get; set; }

        public CreateVirtualInterfaceRequest(string networkId)
        {
            VirtualInterface = new CreateVirtualInterface {NetworkId = networkId};
        }
    }

    [DataContract]
    internal class CreateVirtualInterface
    {
        [DataMember(Name = "network_id")]
        public string NetworkId { get; set; }
    }
}
