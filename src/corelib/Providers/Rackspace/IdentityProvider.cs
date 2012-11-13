using SimpleRestServices.Client;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly IdentityProviderFactory _factory;

        public IdentityProvider(IRestService restService = null, ICache<UserAccess> tokenCache = null)
        {
            _factory = new IdentityProviderFactory(restService, tokenCache);
        }

        public Role[] ListRoles(CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.ListRoles(identity);
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

        public UserAccess Authenticate(CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.Authenticate(identity);
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);

            return provider.AddRoleToUser(userId, roleId, identity);
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
