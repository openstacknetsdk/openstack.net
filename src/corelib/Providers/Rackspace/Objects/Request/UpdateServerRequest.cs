using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class UpdateServerRequest
    {
        [DataMember(Name="server")]
        public ServerUpdateDetails Server { get; set; }

        public UpdateServerRequest()
        {
            Server = new ServerUpdateDetails();
        }

        public UpdateServerRequest(string name, string accessIPv4, string accessIPv6)
        {
            Server = new ServerUpdateDetails{Name = name, AccessIpV4 = accessIPv4, AccessIPv6 = accessIPv6};
        }
    }

    [DataContract]
    internal class ServerUpdateDetails
    {
        [DataMember(Name="name", EmitDefaultValue = true)]
        public string Name { get; set; }

        [DataMember(Name = "accessIPv4", EmitDefaultValue = true)]
        public string AccessIpV4 { get; set; }

        [DataMember(Name = "accessIPv6", EmitDefaultValue = true)]
        public string AccessIPv6 { get; set; }
    }
}
