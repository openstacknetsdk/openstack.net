namespace net.openstack.Core.Domain
{
    public class AccessDetails
    {
        public IdentityToken Token { get; set; }

        public TokenUser User { get; set; }

        public ServiceCatalog[] ServiceCatalog { get; set; }
    }
}