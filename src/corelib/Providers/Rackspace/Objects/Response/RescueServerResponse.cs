using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class RescueServerResponse
    {
        [DataMember(Name = "adminPass")]
        public string AdminPassword { get; set; }
    }
}