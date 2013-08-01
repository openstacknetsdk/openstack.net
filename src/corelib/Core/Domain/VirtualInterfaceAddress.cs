using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class VirtualInterfaceAddress
    {
        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "network_id")]
        public string NetworkId { get; set; }

        [DataMember(Name = "network_label")]
        public string NetworkLabel { get; set; }
    }
}