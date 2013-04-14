using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    public class CreateCloudNetworksDetails
    {
        [DataMember(Name = "cidr")]
        public string Cidr { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }
    }
}
