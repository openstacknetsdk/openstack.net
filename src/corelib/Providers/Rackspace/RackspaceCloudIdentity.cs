using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class RackspaceCloudIdentity : CloudIdentity
    {
        public RackspaceCloudIdentity(){}

        public RackspaceCloudIdentity(CloudIdentity cloudIdentity)
        {
            this.Username = cloudIdentity.Username;
            this.Password = cloudIdentity.Password;
            this.APIKey = cloudIdentity.APIKey;
        }

        public CloudInstance CloudInstance { get; set; }
    }

    public enum CloudInstance
    {
        Default, UK
    }
}
