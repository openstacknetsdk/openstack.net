using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Network
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember(Name = "ip")]
        public Address[] Addresses { get; set; }
    }
}