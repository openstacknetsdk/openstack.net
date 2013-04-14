using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateCloudNetworkRequest
    {
        [DataMember(Name = "network")]
        public CreateCloudNetworksDetails Details { get; set; }
    }
}
