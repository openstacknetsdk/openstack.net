using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListImagesResponse
    {
        public ServerImage[] Images { get; set; }
    }
}