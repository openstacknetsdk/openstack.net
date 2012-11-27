using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateServerImageRequest
    {
        [DataMember(Name = "createImage")]
        public CreateServerImageDetails Details { get; set; }
    }
}