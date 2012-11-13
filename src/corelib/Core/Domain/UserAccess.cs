namespace net.openstack.Core.Domain
{
    public class UserAccess
    {
        public IdentityToken Token { get; set; }

        public TokenUser User { get; set; }

        public ServiceCatalog[] ServiceCatalog { get; set; }
    }
}