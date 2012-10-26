using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class NewServer
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember(Name = "OS-DCF:diskConfig")]
        public string DiskConfig { get; set; }

        [DataMember(Name = "adminPass")]
        public string AdminPassword { get; set; }

        [DataMember]
        public Link[] Links { get; set; }
    }
}