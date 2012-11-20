using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    internal class ListServersResponse
    {
        public ServerDetails[] Servers { get; set; }
    }
}