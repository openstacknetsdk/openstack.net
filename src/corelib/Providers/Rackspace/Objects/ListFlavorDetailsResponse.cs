using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    public class ListFlavorDetailsResponse
    {
        public FlavorDetails[] Flavors { get; set; }
    }
}