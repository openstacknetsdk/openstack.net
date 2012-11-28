using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{

    [DataContract]
    public class User
    {

        [DataMember(Name = "RAX-AUTH:defaultRegion")]
        public string defaultRegion { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public bool enabled { get; set; }

    }

    public class UpdateUserRequest
    {
        public User user { get; set; }
    }
}