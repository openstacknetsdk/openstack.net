using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class RackspaceCloudIdentity : CloudIdentity
    {
        public CloudInstance CloudInstance { get; set; }
    }

    public enum CloudInstance
    {
        US, Lon
    }
}
