using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ServerRebootRequest
    {
        [DataMember(Name= "reboot")]
        public ServerRebootDetails Details { get; set; }
    }
}