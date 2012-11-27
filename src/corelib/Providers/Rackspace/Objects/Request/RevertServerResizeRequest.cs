using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class RevertServerResizeRequest
    {
        [DataMember(Name = "revertResize")]
        public object Details { get; set; }
    }
}