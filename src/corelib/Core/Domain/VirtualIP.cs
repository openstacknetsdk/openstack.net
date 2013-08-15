using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class VirtualIP
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember(Name = "address")]
        public string IPAddress { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string IPVersion { get; set; }
    }
}