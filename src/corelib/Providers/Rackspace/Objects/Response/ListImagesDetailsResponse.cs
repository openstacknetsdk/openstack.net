using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class ListImagesDetailsResponse
    {
        public ServerImageDetails[] Images { get; set; }
    }
}