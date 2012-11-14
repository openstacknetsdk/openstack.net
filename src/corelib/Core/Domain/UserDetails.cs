using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class UserDetails
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Role[] Roles { get; set; }

        [DataMember(Name = "RAX-AUTH:defaultRegion")]
        public string DefaultRegion { get; set; }
    }
}