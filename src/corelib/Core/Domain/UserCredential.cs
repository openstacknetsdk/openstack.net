using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class UserCredential
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "apiKey")]
        public string APIKey { get; set; }
    }
}
