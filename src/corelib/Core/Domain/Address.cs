using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class Address
    {
        [DataMember(Name = "addr")]
        public string Url { get; set; }

        [DataMember]
        public string Version { get; set; }
    }
}