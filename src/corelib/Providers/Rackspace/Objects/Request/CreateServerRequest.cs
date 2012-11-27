using System.Collections.Generic;
using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class CreateServerRequest
    {
        [DataMember(Name = "server")]
        public CreateServerDetails Details { get; set; }
    }

    internal class CreateServerDetails
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "imageRef")]
        public string ImageName { get; set; }

        [DataMember(Name = "flavorRef")]
        public string Flavor { get; set; }

        [DataMember(Name = "OS-DCF:diskConfig")]
        public string DiskConfig { get; set; }

        [DataMember(Name = "metadata", EmitDefaultValue = true)]
        public Dictionary<string, string> Metadata { get; set; }

        public CreateServerDetails()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}
