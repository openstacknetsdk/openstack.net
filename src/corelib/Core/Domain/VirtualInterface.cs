using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class VirtualInterface
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "ip_addresses")]
        public VirtualInterfaceAddress[] Addresses { get; set; }

        [DataMember(Name = "mac_address")]
        public string MACAddress { get; set; }
    }
}
