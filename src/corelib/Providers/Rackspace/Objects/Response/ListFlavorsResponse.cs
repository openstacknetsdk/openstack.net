using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListFlavorsResponse
    {
        public Flavor[] Flavors { get; private set; }
    }
}
