using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ServerResizeRequest
    {
        [DataMember(Name = "resize")]
        public ServerResizeDetails Details { get; set; }
    }
}