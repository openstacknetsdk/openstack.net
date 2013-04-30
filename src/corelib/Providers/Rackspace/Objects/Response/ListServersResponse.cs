using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListServersResponse
    {
        public Server[] Servers { get; set; }
    }
}