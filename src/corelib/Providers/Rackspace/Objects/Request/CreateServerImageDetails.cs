using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateServerImageDetails
    {
        [DataMember(Name = "name")]
        public string ImageName { get; set; }

        [DataMember(Name = "metadata", EmitDefaultValue = true)]
        public Metadata Metadata { get; set; }
    }
}