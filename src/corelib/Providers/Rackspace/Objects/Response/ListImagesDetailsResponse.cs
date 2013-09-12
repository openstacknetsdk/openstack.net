using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListImagesDetailsResponse
    {
        public ServerImage[] Images { get; private set; }
    }
}