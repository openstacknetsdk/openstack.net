using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListImagesResponse
    {
        public SimpleServerImage[] Images { get; private set; }
    }
}
