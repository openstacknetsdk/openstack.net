using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    [DataContract]
    internal class ListServersResponse
    {
        [DataMember(Name = "servers")]
        public Server[] Servers { get; set; }

        [DataMember(Name = "servers_links")]
        public ServerLink[] Links { get; set; }
    }

    [DataContract]
    internal class ServerLink
    {
        [DataMember(Name = "href")]
        public string Link { get; set; }

        [DataMember(Name = "rel")]
        public string Type { get; set; }
    }
}