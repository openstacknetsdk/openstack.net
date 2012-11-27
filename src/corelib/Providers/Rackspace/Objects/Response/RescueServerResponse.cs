using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    public class RescueServerResponse
    {
        [DataMember(Name = "adminPass")]
        public string AdminPassword { get; set; }
    }
}