using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class RescueServerRequest
    {
        [DataMember(Name = "rescue")]
        public string Details { get; set; }
    }
}