using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects
{
    [DataContract]
    internal class AuthDetails
    {
        [DataMember(Name = "passwordCredentials", EmitDefaultValue = true)]
        public Credentials PasswordCredentials { get; set; }

        [DataMember(Name = "RAX-KSKEY:apiKeyCredentials", EmitDefaultValue = true)]
        public Credentials APIKeyCredentials { get; set; }

        [DataMember(Name = "RAX-AUTH:domain", EmitDefaultValue = true)]
        public Domain Domain { get; set; }
    }
}