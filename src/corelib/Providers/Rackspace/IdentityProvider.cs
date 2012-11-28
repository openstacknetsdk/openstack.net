using System.Collections.Generic;
using SimpleRestServices.Client;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace.Objects.Response;

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

        public Role[] GetRolesByUser(CloudIdentity identity, string userId)
        {
            var provider = GetProvider(identity);

            return provider.GetRolesByUser(identity, userId);
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

        public bool UpdateUser(CloudIdentity identity, UpdateUserRequest userRequest)
        {
            var provider = GetProvider(identity);

            return provider.UpdateUser(identity, userRequest);
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
