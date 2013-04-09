using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class AddressDetails
    {
        [DataMember(Name = "addr")]
        internal string Address { get; set; }

        [DataMember]
        public string Version { get; set; }
    }
}