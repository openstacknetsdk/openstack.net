using SimpleRestServices.Client;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;

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

        public Role AddRole(CloudIdentity identity, Role role)
        {
            var provider = GetProvider(identity);
            return provider.AddRole(identity, role);
        }

        public Role GetRole(CloudIdentity identity, string roleId)
        {
            var provider = GetProvider(identity);
            return provider.GetRole(identity, roleId);
        }

        public Role[] GetRolesByUser(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);

            return provider.GetRolesByUser(identity, userId);
        }

        public User[] ListUsers(CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.ListUsers(identity);
        }

        public User GetUserByName(CloudIdentity identity, string name)
        {
            var provider = GetProvider(identity);

            return provider.GetUserByName(identity, name);
        }

        public UserAccess Authenticate(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var provider = GetProvider(identity);

            return provider.Authenticate(identity, forceCacheRefresh);
        }

        public bool AddRoleToUser(CloudIdentity identity, string userId, string roleId)
        {
            var provider = GetProvider(identity);

            return provider.AddRoleToUser(identity, userId, roleId);
        }

        public User GetUser(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);

            return provider.GetUser(identity, userId);
        }

        public User AddUser(CloudIdentity identity, User user)
        {
            var provider = GetProvider(identity);
            return provider.AddUser(identity, user);
        }

        public User UpdateUser(CloudIdentity identity, User user)
        {
            var provider = GetProvider(identity);

            return provider.UpdateUser(identity, user);
        }

        public bool DeleteUser(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);
            return provider.DeleteUser(identity, userId);
        }

        public bool SetUserPassword(CloudIdentity identity, string userId, string password)
        {
            var provider = GetProvider(identity);
            return provider.SetUserPassword(identity, userId, password);
        }

        public UserCredential[] ListUserCredentials(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);
            return provider.ListUserCredentials(identity, userId);
        }

        public UserCredential UpdateUserCredentials(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);
            return provider.UpdateUserCredentials(identity, userId);
        }

        public bool DeleteUserCredentials(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);
            return provider.DeleteUserCredentials(identity, userId);
        }

        public string GetToken(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var provider = GetProvider(identity);

            return provider.GetToken(identity, forceCacheRefresh);
        }

        public bool DeleteRoleFromUser(CloudIdentity identity, string userId, string roleId)
        {
            var provider = GetProvider(identity);

            return provider.DeleteRoleFromUser(identity, userId, roleId);
        }

        public IdentityToken GetTokenInfo(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var provider = GetProvider(identity);

            return provider.GetTokenInfo(identity);
        }

        private IIdentityProvider GetProvider(CloudIdentity identity)
        {
            var rackspaceCloudIdentity = identity as RackspaceCloudIdentity;

            if (rackspaceCloudIdentity == null)
                throw new InvalidCloudIdentityException(string.Format("Invalid Identity object.  Rackspace Identoty service requires an instance of type: {0}", typeof(RackspaceCloudIdentity)));

            return _factory.Get(rackspaceCloudIdentity.CloudInstance);
        }
    }
}
