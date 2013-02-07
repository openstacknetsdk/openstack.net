using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects
{
    [DataContract]
    internal class Credentials
    {
        [DataMember(Name="username", EmitDefaultValue = true)]
        public string Username { get; set; }

        [DataMember(Name="password", EmitDefaultValue = true)]
        public string Password { get; set; }

        [DataMember(Name = "apiKey", EmitDefaultValue = true)]
        public string APIKey { get; set; }
    }
}