using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class RevertServerResizeRequest
    {
        [DataMember(Name = "revertResize")]
        public string Details { get; set; }

        public RevertServerResizeRequest()
        {
            Details = string.Empty;
        }
    }
}