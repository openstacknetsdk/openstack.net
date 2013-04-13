using System;
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

    [DataContract]
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

        [DataMember(Name = "accessIPv4", EmitDefaultValue = true)]
        public string AccessIPv4 { get; set; }

        [DataMember(Name = "accessIPv6", EmitDefaultValue = true)]
        public string AccessIPv6 { get; set; }

        [DataMember(Name = "networks", EmitDefaultValue = true)]
        public NewServerNetwork[] Networks { get; set; }

        [DataMember(Name = "personality", EmitDefaultValue = true)]
        public string Personality { get; set; }

        public CreateServerDetails()
        {
            Metadata = new Dictionary<string, string>();
        }
    }

    [DataContract]
    internal class NewServerNetwork
    {
        [DataMember(Name = "uuid")]
        public Guid Id { get; set; }
    }
}
