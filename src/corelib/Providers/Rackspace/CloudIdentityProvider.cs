using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Caching;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public class CloudIdentityProvider : IExtendedCloudIdentityProvider
    {
        private readonly CloudIdentityProviderFactory _factory;
        private readonly CloudIdentity _defaultIdentity;

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudIdentityProvider"/> class.
        /// </summary>
        public CloudIdentityProvider() : this(null, null, null, null, null)
        {}

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudIdentityProvider"/> class.
        /// </summary>
        /// <param name="defaultIdentity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object.<remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, null, null, null, null)
        { }

        public CloudIdentityProvider(string usInstanceUrlBase, string ukInstanceUrlBase)
            : this(null, null, null, usInstanceUrlBase, ukInstanceUrlBase)
        { }

        public CloudIdentityProvider(CloudIdentity defaultIdentity, string usInstanceUrlBase, string ukInstanceUrlBase)
            : this(defaultIdentity, null, null, usInstanceUrlBase, ukInstanceUrlBase)
        { }

        public CloudIdentityProvider(IRestService restService, ICache<UserAccess> tokenCache, string usInstanceUrlBase, string ukInstanceUrlBase)
            : this(null, restService, tokenCache, usInstanceUrlBase, ukInstanceUrlBase)
        {}

        public CloudIdentityProvider(ICache<UserAccess> tokenCache)
            : this( null, tokenCache)
        { }

        public CloudIdentityProvider(IRestService restService)
            : this(restService, null)
        { }

        public CloudIdentityProvider(IRestService restService, ICache<UserAccess> tokenCache)
            : this(null, restService, tokenCache, null, null)
        { }

        public CloudIdentityProvider(CloudIdentity defaultIdentity, IRestService restService, ICache<UserAccess> tokenCache, string usInstanceUrlBase, string ukInstanceUrlBase)
        {
            _factory = new CloudIdentityProviderFactory(defaultIdentity, restService, tokenCache, usInstanceUrlBase, ukInstanceUrlBase);
            _defaultIdentity = defaultIdentity;
        }

        public IEnumerable<Role> ListRoles(string serviceId = null, string markerId = null, int? limit = null, CloudIdentity identity = null)
        {
            var provider = GetProvider(identity);
            return provider.ListRoles(serviceId, markerId, limit, identity);
        }

        public Role AddRole(string name, string descritpion, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.AddRole(name, descritpion, identity);
        }

        public Role GetRole(string roleId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.GetRole(roleId, identity);
        }

        /// <inheritdoc/>
        public IEnumerable<Role> GetRolesByUser(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");

            var provider = GetProvider(identity);
            return provider.GetRolesByUser(userId, identity: identity);
        }

        /// <inheritdoc/>
        public IEnumerable<User> ListUsers(CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.ListUsers(identity);
        }

        /// <inheritdoc/>
        public User GetUserByName(string name, CloudIdentity identity)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            var provider = GetProvider(identity);
            return provider.GetUserByName(name, identity: identity);
        }

        /// <inheritdoc/>
        public UserAccess Authenticate(CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.Authenticate(identity);
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.AddRoleToUser(userId, roleId, identity: identity);
        }

        /// <inheritdoc/>
        public User GetUser(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");

            var provider = GetProvider(identity);
            return provider.GetUser(userId, identity: identity);
        }

        /// <inheritdoc/>
        public NewUser AddUser(NewUser user, CloudIdentity identity)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("user.Username cannot be null or empty");

            var provider = GetProvider(identity);
            return provider.AddUser(user, identity: identity);
        }

        /// <inheritdoc/>
        public User UpdateUser(User user, CloudIdentity identity)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentException("user.Id cannot be null or empty");

            var provider = GetProvider(identity);
            return provider.UpdateUser(user, identity: identity);
        }

        /// <inheritdoc/>
        public bool DeleteUser(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");

            var provider = GetProvider(identity);
            return provider.DeleteUser(userId, identity: identity);
        }

        public bool SetUserPassword(string userId, string password, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.SetUserPassword(userId, password, identity: identity);
        }

        public bool SetUserPassword(User user, string password, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.SetUserPassword(user, password, identity: identity);
        }

        public bool SetUserPassword(string userId, string username, string password, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.SetUserPassword(userId, username, password, identity: identity);
        }

        public UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.UpdateUserCredentials(user, apiKey, identity: identity);
        }

        public UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.UpdateUserCredentials(userId, username, apiKey, identity: identity);
        }

        /// <inheritdoc/>
        public IEnumerable<UserCredential> ListUserCredentials(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");

            var provider = GetProvider(identity);
            return provider.ListUserCredentials(userId, identity: identity);
        }

        public UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.UpdateUserCredentials(userId, apiKey, identity: identity);
        }

        public bool DeleteUserCredentials(string userId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.DeleteUserCredentials(userId, identity: identity);
        }

        /// <inheritdoc/>
        public IEnumerable<Tenant> ListTenants(CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.ListTenants(identity);
        }

        /// <inheritdoc/>
        public UserAccess GetUserAccess(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var provider = GetProvider(identity);
            return provider.GetUserAccess(identity, forceCacheRefresh);
        }

        /// <inheritdoc/>
        public UserCredential GetUserCredential(string userId, string credentialKey, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (credentialKey == null)
                throw new ArgumentNullException("credentialKey");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(credentialKey))
                throw new ArgumentException("credentialKey cannot be empty");

            var provider = GetProvider(identity);
            return provider.GetUserCredential(userId, credentialKey, identity: identity);
        }

        /// <inheritdoc/>
        public CloudIdentity DefaultIdentity { get { return _defaultIdentity; } }

        /// <inheritdoc/>
        public IdentityToken GetToken(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            var provider = GetProvider(identity);
            return provider.GetToken(identity, forceCacheRefresh);
        }

        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            var provider = GetProvider(identity);
            return provider.DeleteRoleFromUser(userId, roleId, identity: identity);
        }

        private IExtendedCloudIdentityProvider GetProvider(CloudIdentity identity)
        {
            IExtendedCloudIdentityProvider result = _factory.Get(identity);
            if (result == null && identity == null)
                throw new InvalidOperationException("No identity was specified for the request, and no default is available for the provider.");

            return result;
        }
    }
}
