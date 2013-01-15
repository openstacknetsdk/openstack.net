using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{

    [DataContract]
    public class User
    {

        [DataMember(Name = "RAX-AUTH:defaultRegion")]
        public string DefaultRegion { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

    }
}