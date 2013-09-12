using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core.Caching;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace.Objects;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Provides an implementation of <see cref="IIdentityProvider"/> and <see cref="IExtendedCloudIdentityProvider"/>
    /// for operating with Rackspace's Cloud Identity product.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/">OpenStack Identity Service API v2.0 Reference</seealso>
    /// <seealso href="http://docs.rackspace.com/auth/api/v2.0/auth-client-devguide/content/index.html">Rackspace Cloud Identity Client Developer Guide - API v2.0</seealso>
    public class CloudIdentityProvider : IExtendedCloudIdentityProvider, IIdentityProvider
    {
        private readonly CloudIdentityProviderFactory _factory;
        private readonly CloudIdentity _defaultIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, and the default <see cref="CloudInstance"/> base URLs,
        /// the REST service implementation, and token cache.
        /// </summary>
        public CloudIdentityProvider() : this(null, null, null, null, null)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with the specified default identity, and the default <see cref="CloudInstance"/>
        /// base URLs, the REST service implementation, and token cache.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, null, null, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the specified <see cref="CloudInstance"/> base URLs,
        /// and the default REST service implementation and token cache.
        /// </summary>
        /// <param name="usInstanceUrlBase">The base URL for the <see cref="CloudInstance.US"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        /// <param name="ukInstanceUrlBase">The base URL for the <see cref="CloudInstance.UK"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://lon.identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(string usInstanceUrlBase, string ukInstanceUrlBase)
            : this(null, null, null, usInstanceUrlBase, ukInstanceUrlBase)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with the specified default identity and <see cref="CloudInstance"/> base URLs,
        /// and the default REST service implementation and token cache.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="usInstanceUrlBase">The base URL for the <see cref="CloudInstance.US"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        /// <param name="ukInstanceUrlBase">The base URL for the <see cref="CloudInstance.UK"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://lon.identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity, string usInstanceUrlBase, string ukInstanceUrlBase)
            : this(defaultIdentity, null, null, usInstanceUrlBase, ukInstanceUrlBase)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, and the specified <see cref="CloudInstance"/> base URLs,
        /// REST service implementation, and token cache.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        /// <param name="usInstanceUrlBase">The base URL for the <see cref="CloudInstance.US"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        /// <param name="ukInstanceUrlBase">The base URL for the <see cref="CloudInstance.UK"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://lon.identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(IRestService restService, ICache<UserAccess> tokenCache, string usInstanceUrlBase, string ukInstanceUrlBase)
            : this(null, restService, tokenCache, usInstanceUrlBase, ukInstanceUrlBase)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the default <see cref="CloudInstance"/> base URLs,
        /// the default REST service implementation, and the specified token cache.
        /// </summary>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        public CloudIdentityProvider(ICache<UserAccess> tokenCache)
            : this( null, tokenCache)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the default <see cref="CloudInstance"/> base URLs,
        /// the specified REST service implementation, and the <see cref="UserAccessCache.Instance"/>
        /// token cache.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        public CloudIdentityProvider(IRestService restService)
            : this(restService, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the default <see cref="CloudInstance"/> base URLs,
        /// and the specified REST service implementation and token cache.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        public CloudIdentityProvider(IRestService restService, ICache<UserAccess> tokenCache)
            : this(null, restService, tokenCache, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// using the provided values.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        /// <param name="usInstanceUrlBase">The base URL for the <see cref="CloudInstance.US"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        /// <param name="ukInstanceUrlBase">The base URL for the <see cref="CloudInstance.UK"/> cloud instance. If this value is <c>null</c>, the provider will use <c>https://lon.identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity, IRestService restService, ICache<UserAccess> tokenCache, string usInstanceUrlBase, string ukInstanceUrlBase)
        {
            _factory = new CloudIdentityProviderFactory(defaultIdentity, restService, tokenCache, usInstanceUrlBase, ukInstanceUrlBase);
            _defaultIdentity = defaultIdentity;
        }

        /// <inheritdoc/>
        public IEnumerable<Role> ListRoles(string serviceId = null, string markerId = null, int? limit = null, CloudIdentity identity = null)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");

            var provider = GetProvider(identity);
            return provider.ListRoles(serviceId, markerId, limit, identity);
        }

        /// <inheritdoc/>
        public Role AddRole(string name, string description, CloudIdentity identity)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            var provider = GetProvider(identity);
            return provider.AddRole(name, description, identity);
        }

        /// <inheritdoc/>
        public Role GetRole(string roleId, CloudIdentity identity)
        {
            if (roleId == null)
                throw new ArgumentNullException("roleId");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentException("roleId cannot be empty");

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

        /// <inheritdoc/>
        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (roleId == null)
                throw new ArgumentNullException("roleId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentException("roleId cannot be empty");

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
            if (user.Id != null)
                throw new InvalidOperationException("user.Id must be null");

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

        /// <inheritdoc/>
        public bool SetUserPassword(string userId, string password, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password cannot be empty");

            var provider = GetProvider(identity);
            return provider.SetUserPassword(userId, password, identity: identity);
        }

        /// <inheritdoc/>
        public bool SetUserPassword(User user, string password, CloudIdentity identity)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password cannot be empty");
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentException("user.Id cannot be null or empty");
            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("user.Username cannot be null or empty");

            var provider = GetProvider(identity);
            return provider.SetUserPassword(user, password, identity: identity);
        }

        /// <inheritdoc/>
        public bool SetUserPassword(string userId, string username, string password, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (username == null)
                throw new ArgumentNullException("username");
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username cannot be empty");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password cannot be empty");

            var provider = GetProvider(identity);
            return provider.SetUserPassword(userId, username, password, identity: identity);
        }

        /// <inheritdoc/>
        public UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("apiKey cannot be empty");
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentException("user.Id cannot be null or empty");
            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("user.Username cannot be null or empty");

            var provider = GetProvider(identity);
            return provider.UpdateUserCredentials(user, apiKey, identity: identity);
        }

        /// <inheritdoc/>
        public UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (username == null)
                throw new ArgumentNullException("username");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username cannot be empty");
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("apiKey cannot be empty");

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

        /// <inheritdoc/>
        public UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("apiKey cannot be empty");

            var provider = GetProvider(identity);
            return provider.UpdateUserCredentials(userId, apiKey, identity: identity);
        }

        /// <inheritdoc/>
        public bool DeleteUserCredentials(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");

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

        /// <inheritdoc/>
        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (roleId == null)
                throw new ArgumentNullException("roleId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentException("roleId cannot be empty");

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
