using net.openstack.Providers.Rackspace.Objects;

namespace net.openstack.Providers.Rackspace
{
    public class RackspaceImpersonationIdentity : RackspaceCloudIdentity
    {
        public RackspaceCloudIdentity UserToImpersonate { get; set; }
    }
}
