using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListFlavorDetailsResponse
    {
        public FlavorDetails[] Flavors { get; set; }
    }
}