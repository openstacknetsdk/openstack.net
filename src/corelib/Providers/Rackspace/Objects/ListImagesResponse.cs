using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    public class ListImagesResponse
    {
        public ServerImage[] Images { get; set; }
    }
}