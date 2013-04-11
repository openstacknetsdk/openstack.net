using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ConfirmServerResizeRequest
    {
        [DataMember(Name = "confirmResize")]
        public string Details { get; set; }

        public ConfirmServerResizeRequest()
        {
            Details = string.Empty;
        }
    }
}