using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    internal class ServerResizeDetails
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "flavorRef")]
        public string Flavor { get; set; }

        [DataMember(Name = "OS-DCF:diskConfig", EmitDefaultValue = true)]
        public string DiskConfig { get; set; }
    }
}