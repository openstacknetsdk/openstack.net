using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class RackspaceCloudIdentity : CloudIdentity
    {
        public RackspaceCloudIdentity()
        {
            CloudInstance = CloudInstance.Default;
        }

        public RackspaceCloudIdentity(CloudIdentity cloudIdentity) : this()
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
