using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ServerRebuildRequest
    {   
        [DataMember(Name = "rebuild")]
        public ServerRebuildDetails Details { get; set; }
    }
}