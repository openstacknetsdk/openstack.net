namespace net.openstack.Core.Domain
{
    public class ImpersonationIdentity : CloudIdentity
    {
        public CloudIdentity UserToImpersonate { get; set; }
    }
}
