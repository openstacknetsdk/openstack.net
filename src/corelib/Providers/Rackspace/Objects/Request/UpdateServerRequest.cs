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

        public UpdateServerRequest(string name, string ipV4Address, string ipV6Address)
        {
            Server = new ServerUpdateDetails{Name = name, IPv4Address = ipV4Address, IPv6Address = ipV6Address};
        }
    }

    [DataContract]
    internal class ServerUpdateDetails
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "accessIPv4")]
        public string IPv4Address { get; set; }

        [DataMember(Name = "accessIPv6")]
        public string IPv6Address { get; set; }
    }
}
