using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    public class ListImagesDetailsResponse
    {
        public ServerImageDetails[] Images { get; set; }
    }
}