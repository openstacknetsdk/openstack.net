using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class UnrescueServerRequest
    {
        [DataMember(Name = "unrescue")]
        public string Details { get; set; }
    }
}