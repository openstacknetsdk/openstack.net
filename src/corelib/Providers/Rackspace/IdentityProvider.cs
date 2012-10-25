using System;
using net.openstack.corelib.Providers.Rackspace.Objects;
using net.openstack.corelib.Web;

namespace net.openstack.corelib.Providers.Rackspace
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly IdentityProviderFactory _factory;

        public IdentityProvider()
        {
            _factory = new IdentityProviderFactory();
        }
        public IdentityProvider(IRestService restService, ICache<IdentityToken> tokenCache)
        {
            _factory = new IdentityProviderFactory();
        }

        public Role[] GetAllRoles(CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.GetAllRoles(identity);
        }

        public Role[] GetRolesByUser(string userId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.GetRolesByUser(userId, identity);
        }

        public User GetUserByName(string name, CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.GetUserByName(name, identity);
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.AddRoleToUser(userId, roleId, identity);
        }

        public string GetUserImpersonationToken(string userName, CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.GetUserImpersonationToken(userName, identity);
        }

        public string GetToken(CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.GetToken(identity);
        }

        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.DeleteRoleFromUser(userId, roleId, identity);
        }

        public IdentityToken GetTokenInfo(CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.GetTokenInfo(identity);
        }

        private IIdentityProvider GetProvider(CloudIdentity identity)
        {
            return _factory.Get(identity.Region);
        }
    }
}
