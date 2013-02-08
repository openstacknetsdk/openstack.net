using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class NewUser
    {
        [DataMember(Name = "OS-KSADM:password")]
        public string Password { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = true)]
        public string Id { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }
}
