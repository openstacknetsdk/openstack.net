using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class AddressDetails
    {
        [DataMember(Name = "addr")]
        public string Address { get; set; }

        [DataMember]
        public string Version { get; set; }
    }
}