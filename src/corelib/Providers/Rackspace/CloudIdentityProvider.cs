using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Caching;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace.Objects;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CancellationToken = System.Threading.CancellationToken;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Provides an implementation of <see cref="IIdentityProvider"/> and <see cref="IExtendedCloudIdentityProvider"/>
    /// for operating with Rackspace's Cloud Identity product.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/">OpenStack Identity Service API v2.0 Reference</seealso>
    /// <seealso href="http://docs.rackspace.com/auth/api/v2.0/auth-client-devguide/content/index.html">Rackspace Cloud Identity Client Developer Guide - API v2.0</seealso>
    /// <threadsafety static="true" instance="false"/>
    public class CloudIdentityProvider : ProviderBase<IIdentityProvider>, IExtendedCloudIdentityProvider, IIdentityProvider, IIdentityService
    {
        /// <summary>
        /// This is the backing field for the <see cref="TokenCache"/> property.
        /// </summary>
        private readonly ICache<UserAccess> _userAccessCache;

        /// <summary>
        /// This is the backing field for the <see cref="UrlBase"/> property.
        /// </summary>
        private readonly Uri _urlBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, and the default base URL, REST service implementation,
        /// and token cache.
        /// </summary>
        public CloudIdentityProvider() : this(null, null, null, null)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with the specified default identity, and the default base URL, REST service
        /// implementation, and token cache.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity)
            : this(defaultIdentity, null, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the specified base URL, and the default REST service
        /// implementation and token cache.
        /// </summary>
        /// <param name="urlBase">The base URL for the cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(Uri urlBase)
            : this(null, null, null, urlBase)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with the specified default identity and base URL, and the default REST service
        /// implementation and token cache.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="urlBase">The base URL for the cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity, Uri urlBase)
            : this(defaultIdentity, null, null, urlBase)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, and the specified base URL, REST service
        /// implementation, and token cache.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        /// <param name="urlBase">The base URL for the cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(IRestService restService, ICache<UserAccess> tokenCache, Uri urlBase)
            : this(null, restService, tokenCache, urlBase)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the default base URL, the default REST service
        /// implementation, and the specified token cache.
        /// </summary>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        public CloudIdentityProvider(ICache<UserAccess> tokenCache)
            : this( null, tokenCache)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the default base URL, the specified REST service
        /// implementation, and the <see cref="UserAccessCache.Instance"/>
        /// token cache.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        public CloudIdentityProvider(IRestService restService)
            : this(restService, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// with no default identity, the default base URL, and the specified REST service
        /// implementation and token cache.
        /// </summary>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        public CloudIdentityProvider(IRestService restService, ICache<UserAccess> tokenCache)
            : this(null, restService, tokenCache, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudIdentityProvider"/> class
        /// using the provided values.
        /// </summary>
        /// <param name="defaultIdentity">The default identity to use for calls that do not explicitly specify an identity. If this value is <c>null</c>, no default identity is available so all calls must specify an explicit identity.</param>
        /// <param name="restService">The implementation of <see cref="IRestService"/> to use for executing REST requests. If this value is <c>null</c>, the provider will use a new instance of <see cref="JsonRestServices"/>.</param>
        /// <param name="tokenCache">The cache to use for caching user access tokens. If this value is <c>null</c>, the provider will use <see cref="UserAccessCache.Instance"/>.</param>
        /// <param name="urlBase">The base URL for the cloud instance. If this value is <c>null</c>, the provider will use <c>https://identity.api.rackspacecloud.com</c>.</param>
        public CloudIdentityProvider(CloudIdentity defaultIdentity, IRestService restService, ICache<UserAccess> tokenCache, Uri urlBase)
            : base(defaultIdentity, null, null, restService)
        {
            _userAccessCache = tokenCache ?? UserAccessCache.Instance;
            _urlBase = urlBase ?? new Uri("https://identity.api.rackspacecloud.com");
        }

        /// <summary>
        /// Gets the cache to use for caching user access tokens.
        /// </summary>
        protected ICache<UserAccess> TokenCache
        {
            get
            {
                return _userAccessCache;
            }
        }

        /// <summary>
        /// Gets the base URL for the cloud instance.
        /// </summary>
        protected Uri UrlBase
        {
            get
            {
                return _urlBase;
            }
        }

        #region Roles

        /// <inheritdoc/>
        public virtual IEnumerable<Role> ListRoles(string serviceId = null, int? marker = null, int? limit = null, CloudIdentity identity = null)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException("limit");
            CheckIdentity(identity);

            var parameters = BuildOptionalParameterList(new Dictionary<string, string>
                {
                    {"serviceId", serviceId},
                    {"marker", !marker.HasValue ? null : marker.Value.ToString()},
                    {"limit", !limit.HasValue ? null : limit.Value.ToString()},
                });

            var response = ExecuteRESTRequest<RolesResponse>(identity, new Uri(UrlBase, "/v2.0/OS-KSADM/roles"), HttpMethod.GET, queryStringParameter: parameters);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        /// <inheritdoc/>
        public virtual Role AddRole(string name, string description, CloudIdentity identity)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");
            CheckIdentity(identity);

            var response = ExecuteRESTRequest<RoleResponse>(identity, new Uri(UrlBase, "/v2.0/OS-KSADM/roles"), HttpMethod.POST, new AddRoleRequest(new Role(name, description)));

            if (response == null || response.Data == null)
                return null;

            return response.Data.Role;
        }

        /// <inheritdoc/>
        public virtual Role GetRole(string roleId, CloudIdentity identity)
        {
            if (roleId == null)
                throw new ArgumentNullException("roleId");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentException("roleId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/OS-KSADM/roles/{0}", roleId);
            var response = ExecuteRESTRequest<RoleResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Role;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<Role> GetRolesByUser(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/users/{0}/roles", userId);
            var response = ExecuteRESTRequest<RolesResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Roles;
        }

        /// <inheritdoc/>
        public virtual bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (roleId == null)
                throw new ArgumentNullException("roleId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentException("roleId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest(identity, new Uri(UrlBase, urlPath), HttpMethod.PUT);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || (response.StatusCode >= HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.Conflict))
                return false;

            return true;
        }

        /// <inheritdoc/>
        public virtual bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (roleId == null)
                throw new ArgumentNullException("roleId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentException("roleId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/users/{0}/roles/OS-KSADM/{1}", userId, roleId);
            var response = ExecuteRESTRequest(identity, new Uri(UrlBase, urlPath), HttpMethod.DELETE);

            if (response != null && response.StatusCode == HttpStatusCode.NoContent)
                return true;

            return false;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<User> ListUsersByRole(string roleId, bool? enabled = null, int? marker = null, int? limit = null, CloudIdentity identity = null)
        {
            if (limit < 0 || limit > 1000)
                throw new ArgumentOutOfRangeException("limit");

            CheckIdentity(identity);

            var parameters = BuildOptionalParameterList(new Dictionary<string, string>
                {
                    {"enabled", !enabled.HasValue ? null : enabled.Value ? "true" : "false"},
                    {"marker", !marker.HasValue ? null : marker.Value.ToString()},
                    {"limit", !limit.HasValue ? null : limit.Value.ToString()},
                });

            var urlPath = string.Format("/v2.0/OS-KSADM/roles/{0}/RAX-AUTH/users", roleId);
            var response = ExecuteRESTRequest<UsersResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.GET, queryStringParameter: parameters);

            if (response == null || response.Data == null)
                return null;
            // Due to the fact the sometimes the API returns a JSON array of users and sometimes it returns a single JSON user object.  
            // Therefore if we get a null data object (which indicates that the deserializer could not parse to an array) we need to try and parse as a single User object.
            if (response.Data.Users == null)
            {
                var userResponse = JsonConvert.DeserializeObject<UserResponse>(response.RawBody);

                if (response == null || response.Data == null)
                    return null;

                return new[] { userResponse.User };
            }

            return response.Data.Users;
        }

        #endregion

        #region Credentials

        /// <inheritdoc/>
        public virtual bool SetUserPassword(string userId, string password, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (password == null)
                throw new ArgumentNullException("password");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password cannot be empty");
            CheckIdentity(identity);

            var user = GetUser(userId, identity);

            return SetUserPassword(user, password, identity);
        }

        /// <inheritdoc/>
        public virtual bool SetUserPassword(User user, string password, CloudIdentity identity)
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
            CheckIdentity(identity);

            return SetUserPassword(user.Id, user.Username, password, identity);
        }

        /// <inheritdoc/>
        public virtual bool SetUserPassword(string userId, string username, string password, CloudIdentity identity)
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
            CheckIdentity(identity);

            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials", userId);
            var request = new SetPasswordRequest(username, password);
            var response = ExecuteRESTRequest<PasswordCredentialResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.POST, request);

            if (response == null || response.StatusCode != HttpStatusCode.Created || response.Data == null)
                return false;

            return response.Data.PasswordCredential.Password.Equals(password);
        }

        /// <inheritdoc/>
        public virtual IEnumerable<UserCredential> ListUserCredentials(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials", userId);
            var response = ExecuteRESTRequest(identity, new Uri(UrlBase, urlPath), HttpMethod.GET);

            if (response == null || string.IsNullOrEmpty(response.RawBody))
                return null;

            var jObject = JObject.Parse(response.RawBody);
            var credsArray = (JArray)jObject["credentials"];
            var creds = new List<UserCredential>();

            foreach (JObject jToken in credsArray)
            {
                foreach (JProperty property in jToken.Properties())
                {
                    var cred = (JObject)property.Value;
                    creds.Add(new UserCredential(property.Name, cred["username"].ToString(), cred["apiKey"].ToString())); 
                }
                   
            }
            
            return creds.ToArray();
        }

        /// <inheritdoc/>
        public virtual UserCredential GetUserCredential(string userId, string credentialKey, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (credentialKey == null)
                throw new ArgumentNullException("credentialKey");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(credentialKey))
                throw new ArgumentException("credentialKey cannot be empty");
            CheckIdentity(identity);

            var creds = ListUserCredentials(userId, identity);

            var cred = creds.FirstOrDefault(c => c.Name.Equals(credentialKey, StringComparison.OrdinalIgnoreCase));

            return cred;
        }

        /// <inheritdoc/>
        public new virtual CloudIdentity DefaultIdentity { get { return base.DefaultIdentity; } }

        /// <inheritdoc/>
        public virtual UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("apiKey cannot be empty");
            CheckIdentity(identity);

            var user = GetUser(userId, identity);

            return UpdateUserCredentials(user, apiKey, identity);
        }

        /// <inheritdoc/>
        public virtual UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity)
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
            CheckIdentity(identity);

            return UpdateUserCredentials(user.Id, user.Username, apiKey, identity);
        }

        /// <inheritdoc/>
        public virtual UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity)
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
            CheckIdentity(identity);

            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials/RAX-KSKEY:apiKeyCredentials", userId);
            var request = new UpdateUserCredentialRequest(username, apiKey);
            var response = ExecuteRESTRequest<UserCredentialResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.POST, request);

            if (response == null || response.Data == null)
                return null;

            return response.Data.UserCredential;
        }

        /// <inheritdoc/>
        public virtual bool DeleteUserCredentials(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("v2.0/users/{0}/OS-KSADM/credentials/RAX-KSKEY:apiKeyCredentials", userId);
            var response = ExecuteRESTRequest(identity, new Uri(UrlBase, urlPath), HttpMethod.DELETE);

            if (response == null || response.StatusCode != HttpStatusCode.OK)
                return false;

            return true;
        }

        #endregion

        #region Users

        /// <inheritdoc/>
        public virtual IEnumerable<User> ListUsers(CloudIdentity identity)
        {
            CheckIdentity(identity);

            var response = ExecuteRESTRequest<UsersResponse>(identity, new Uri(UrlBase, "/v2.0/users"), HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            // Due to the fact the sometimes the API returns a JSON array of users and sometimes it returns a single JSON user object.  
            // Therefore if we get a null data object (which indicates that the deserializer could not parse to an array) we need to try and parse as a single User object.
            if(response.Data.Users == null)
            {
                var userResponse = JsonConvert.DeserializeObject<UserResponse>(response.RawBody);

                if (response == null || response.Data == null)
                    return null;

                return new[] {userResponse.User};
            }

            return response.Data.Users;
        }

        /// <inheritdoc/>
        public virtual User GetUserByName(string name, CloudIdentity identity)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/users/?name={0}", name);
            var response = ExecuteRESTRequest<UserResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.User;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<User> GetUsersByEmail(string email, CloudIdentity identity)
        {
            if (email == null)
                throw new ArgumentNullException("email");
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("email cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/users/?email={0}", EncodeDecodeProvider.Default.UrlEncode(email));
            var response = ExecuteRESTRequest<UsersResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Users;
        }

        /// <inheritdoc/>
        public virtual User GetUser(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("v2.0/users/{0}", userId);

            var response = ExecuteRESTRequest<UserResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.GET);

            return response.Data.User;
        }

        /// <inheritdoc/>
        public virtual NewUser AddUser(NewUser newUser, CloudIdentity identity)
        {
            if (newUser == null)
                throw new ArgumentNullException("newUser");
            if (string.IsNullOrEmpty(newUser.Username))
                throw new ArgumentException("newUser.Username cannot be null or empty");
            if (newUser.Id != null)
                throw new InvalidOperationException("newUser.Id must be null");
            CheckIdentity(identity);

            var response = ExecuteRESTRequest<NewUserResponse>(identity, new Uri(UrlBase, "/v2.0/users"), HttpMethod.POST, new AddUserRequest(newUser));

            if (response == null || response.Data == null)
                return null;

            // If the user specifies a password, then the password will not be in the response, so we need to fill it in on the return object.
            if (string.IsNullOrEmpty(response.Data.NewUser.Password))
                response.Data.NewUser.Password = newUser.Password;

            return response.Data.NewUser;
        }

        /// <inheritdoc/>
        public virtual User UpdateUser(User user, CloudIdentity identity)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentException("user.Id cannot be null or empty");
            CheckIdentity(identity);

            var urlPath = string.Format("v2.0/users/{0}", user.Id);

            var updateUserRequest = new UpdateUserRequest(user);
            var response = ExecuteRESTRequest<UserResponse>(identity, new Uri(UrlBase, urlPath), HttpMethod.POST, updateUserRequest);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || response.Data == null || (response.StatusCode >= HttpStatusCode.BadRequest && response.StatusCode != HttpStatusCode.Conflict))
                return null;

            return response.Data.User;
        }

        /// <inheritdoc/>
        public virtual bool DeleteUser(string userId, CloudIdentity identity)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be empty");
            CheckIdentity(identity);

            var urlPath = string.Format("/v2.0/users/{0}", userId);
            var response = ExecuteRESTRequest(identity, new Uri(UrlBase, urlPath), HttpMethod.DELETE);

            if (response != null && response.StatusCode == HttpStatusCode.NoContent)
                return true;

            return false;
        }

        #endregion

        #region Tenants

        /// <inheritdoc/>
        public virtual IEnumerable<Tenant> ListTenants(CloudIdentity identity)
        {
            CheckIdentity(identity);

            var response = ExecuteRESTRequest<TenantsResponse>(identity, new Uri(UrlBase, "v2.0/tenants"), HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Tenants;
        }

        #endregion

        #region Token and Authentication

        /// <inheritdoc/>
        public virtual IdentityToken GetToken(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            CheckIdentity(identity);

            var auth = GetUserAccess(identity, forceCacheRefresh);

            if (auth == null || auth.Token == null)
                return null;

            return auth.Token;
        }

        /// <inheritdoc/>
        public virtual Task<IdentityToken> GetTokenAsync(CloudIdentity identity, CancellationToken cancellationToken)
        {
            return GetUserAccessAsync(identity, false, cancellationToken)
                .ContinueWith(task => task.Result.Token, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <inheritdoc/>
        public virtual UserAccess Authenticate(CloudIdentity identity)
        {
            CheckIdentity(identity);

            return GetUserAccess(identity, true);
        }

        /// <inheritdoc/>
        public virtual Task<UserAccess> AuthenticateAsync(CloudIdentity identity, CancellationToken cancellationToken)
        {
            CheckIdentity(identity);
            return GetUserAccessAsync(identity, true, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual UserAccess GetUserAccess(CloudIdentity identity, bool forceCacheRefresh = false)
        {
            CheckIdentity(identity);

            if (identity == null)
                identity = DefaultIdentity;

            if (identity is RackspaceImpersonationIdentity)
                return Impersonate(identity as RackspaceImpersonationIdentity, forceCacheRefresh);

            var rackspaceCloudIdentity = identity as RackspaceCloudIdentity;

            if (rackspaceCloudIdentity == null)
                rackspaceCloudIdentity = new RackspaceCloudIdentity(identity);

            var userAccess = TokenCache.Get(string.Format("{0}:{1}/{2}", UrlBase, rackspaceCloudIdentity.Domain, rackspaceCloudIdentity.Username), () =>
                            {
                                var auth = new AuthRequest(identity);
                                var response = ExecuteRESTRequest<AuthenticationResponse>(identity, new Uri(UrlBase, "/v2.0/tokens"), HttpMethod.POST, auth, isTokenRequest: true);


                                if (response == null || response.Data == null || response.Data.UserAccess == null || response.Data.UserAccess.Token == null)
                                    return null;

                                return response.Data.UserAccess;
                            }, forceCacheRefresh);

            return userAccess;
        }

        /// <inheritdoc/>
        public virtual Task<UserAccess> GetUserAccessAsync(CloudIdentity identity, bool forceCacheRefresh, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => GetUserAccess(identity, forceCacheRefresh), cancellationToken);
        }

        /// <summary>
        /// Gets the authentication token for the specified impersonation identity. If necessary, the
        /// identity is authenticated on the server to obtain a token.
        /// </summary>
        /// <remarks>
        /// If <paramref name="forceCacheRefresh"/> is <c>false</c> and a cached <see cref="IdentityToken"/>
        /// is available for the specified <paramref name="identity"/>, this method may return the cached
        /// value without performing an authentication against the server. If <paramref name="forceCacheRefresh"/>
        /// is <c>true</c>, this method always authenticates the identity with the server.
        /// </remarks>
        /// <param name="identity">The identity of the user to authenticate. If this value is <c>null</c>, the authentication is performed with the <see cref="DefaultIdentity"/>.</param>
        /// <param name="forceCacheRefresh">If <c>true</c>, the user is always authenticated against the server; otherwise a cached <see cref="IdentityToken"/> may be returned.</param>
        /// <returns>The user's authentication token.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="identity"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If the provider does not support the given <paramref name="identity"/> type.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the authentication request failed.</exception>
        protected virtual UserAccess Impersonate(RackspaceImpersonationIdentity identity, bool forceCacheRefresh)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");

            var impToken = TokenCache.Get(string.Format("{0}:{1}/imp/{2}/{3}", UrlBase, identity.Username, identity.UserToImpersonate.Domain == null ? "none" : identity.UserToImpersonate.Domain.Name, identity.UserToImpersonate.Username), () =>
            {
                const string urlPath = "/v2.0/RAX-AUTH/impersonation-tokens";
                var request = BuildImpersonationRequestJson(identity.UserToImpersonate.Username, 600);
                var parentIdentity = new RackspaceCloudIdentity(identity);
                var response = ExecuteRESTRequest<UserImpersonationResponse>(parentIdentity, new Uri(UrlBase, urlPath), HttpMethod.POST, request);
                if (response == null || response.Data == null || response.Data.UserAccess == null)
                    return null;

                IdentityToken impersonationToken = response.Data.UserAccess.Token;
                if (impersonationToken == null)
                    return null;

                var userAccess = ValidateToken(impersonationToken.Id, identity: parentIdentity);
                if (userAccess == null)
                    return null;

                var endpoints = ListEndpoints(impersonationToken.Id, parentIdentity);

                var serviceCatalog = BuildServiceCatalog(endpoints);

                return new UserAccess(userAccess.Token, userAccess.User, serviceCatalog);
            }, forceCacheRefresh);

            return impToken;
        }

        /// <summary>
        /// Converts a collection of <see cref="ExtendedEndpoint"/> objects describing service endpoints
        /// for an impersonated identity to a collection of <see cref="ServiceCatalog"/> objects used by
        /// the provider implementations for endpoint resolution.
        /// </summary>
        /// <param name="endpoints">A collection of <see cref="ExtendedEndpoint"/> objects describing the available endpoints.</param>
        /// <returns>A collection of <see cref="ServiceCatalog"/> objects describing the same endpoints as <paramref name="endpoints"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="endpoints"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="endpoints"/> contains any <c>null</c> values.</exception>
        protected virtual ServiceCatalog[] BuildServiceCatalog(IEnumerable<ExtendedEndpoint> endpoints)
        {
            if (endpoints == null)
                throw new ArgumentNullException("endpoints");
            if (endpoints.Contains(null))
                throw new ArgumentException("endpoints cannot contain any null values", "endpoints");

            var serviceCatalog = new List<ServiceCatalog>();
            var services = endpoints.Select(e => Tuple.Create(e.Type, e.Name)).Distinct();

            foreach (var service in services)
            {
                string type = service.Item1;
                string name = service.Item2;
                IEnumerable<ExtendedEndpoint> serviceEndpoints = endpoints.Where(endpoint => string.Equals(type, endpoint.Type, StringComparison.OrdinalIgnoreCase) && string.Equals(name, endpoint.Name, StringComparison.OrdinalIgnoreCase));
                serviceCatalog.Add(new ServiceCatalog(name, type, serviceEndpoints.ToArray()));
            }

            return serviceCatalog.ToArray();
        }

        /// <inheritdoc/>
        public virtual UserAccess ValidateToken(string token, string tenantId = null, CloudIdentity identity = null)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token cannot be empty");

            var queryStringParameters = BuildOptionalParameterList(new Dictionary<string, string>
                {
                    {"belongsTo", tenantId}
                });
            
            var response = ExecuteRESTRequest<AuthenticationResponse>(identity, new Uri(UrlBase, string.Format("/v2.0/tokens/{0}", token)), HttpMethod.GET, queryStringParameter: queryStringParameters);


            if (response == null || response.Data == null || response.Data.UserAccess == null || response.Data.UserAccess.Token == null)
                return null;

            return response.Data.UserAccess;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<ExtendedEndpoint> ListEndpoints(string token, CloudIdentity identity = null)
        {
            if (token == null)
                throw new ArgumentNullException("token");
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token cannot be empty");

            var response = ExecuteRESTRequest<ListEndpointsResponse>(identity, new Uri(UrlBase, string.Format("/v2.0/tokens/{0}/endpoints", token)), HttpMethod.GET);


            if (response == null || response.Data == null)
                return null;

            return response.Data.Endpoints;
        }

        /// <summary>
        /// Constructs the JSON representation used for an impersonation request.
        /// </summary>
        /// <param name="userName">The username of the user to impersonate.</param>
        /// <param name="expirationInSeconds">The time until the impersonation token will expire.</param>
        /// <returns>A <see cref="JObject"/> representing the JSON body of the impersonation request.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="userName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="userName"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="expirationInSeconds"/> is less than or equal to 0.</exception>
        protected virtual JObject BuildImpersonationRequestJson(string userName, int expirationInSeconds)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("userName cannot be empty");
            if (expirationInSeconds <= 0)
                throw new ArgumentOutOfRangeException("expirationInSeconds");

            return new JObject(
                new JProperty("RAX-AUTH:impersonation", new JObject(
                    new JProperty("user", new JObject(
                        new JProperty("username", userName),
                        new JProperty("expire-in-seconds", expirationInSeconds))))));
        }

        #endregion
    }
}
