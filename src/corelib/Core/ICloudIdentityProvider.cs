using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface ICloudIdentityProvider
    {
        /// <summary>
        /// Authenticates the user for the specified identity. <remarks>NOTE: This method always authenticates to the server and never from cache.</remarks>
        /// </summary>
        /// <param name="identity">The user <see cref="CloudIdentity"/>.</param>
        /// <returns>The <see cref="UserAccess"/> data containing the authentication token, service catalog and user data /></returns>
        UserAccess Authenticate(CloudIdentity identity = null);

        /// <summary>
        /// Gets the authentication token for the give user. <remarks>NOTE: Unless overwritten, this will first check the cache for existance and expiration before checking for fresh data from the server</remarks>
        /// </summary>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <param name="forceCacheRefresh">if set to <c>true</c> [force cache refresh].</param>
        /// <returns>The users authentication token</returns>
        string GetToken(CloudIdentity identity = null, bool forceCacheRefresh = false);

        /// <summary>
        /// Gets the full <see cref="IdentityToken"/> details. <remarks>NOTE: Unless overwritten, this will first check the cache for existance and expiration before checking for fresh data from the server</remarks>
        /// </summary>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <param name="forceCacheRefresh">if set to <c>true</c> [force cache refresh].</param>
        /// <returns>The users <see cref="IdentityToken"/></returns>
        IdentityToken GetTokenInfo(CloudIdentity identity = null, bool forceCacheRefresh = false);

        /// <summary>
        /// Gets all roles associated to the specified user
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Role"/></returns>
        IEnumerable<Role> GetRolesByUser(string userId, CloudIdentity identity = null);

        /// <summary>
        /// Lists all the <see cref="User"/> for the account.
        /// </summary>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="User"/></returns>
        IEnumerable<User> ListUsers(CloudIdentity identity = null);

        /// <summary>
        /// Gets the details for the specified username
        /// </summary>
        /// <param name="name">The username.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="User"/> details.</returns>
        User GetUserByName(string name, CloudIdentity identity = null);

        /// <summary>
        /// Retrieves the details for the specified user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="User"/> details.</returns>
        User GetUser(string id, CloudIdentity identity = null);

        /// <summary>
        /// Adds a user to the account.
        /// </summary>
        /// <param name="user">The new <see cref="NewUser"/> details.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="NewUser"/>.</returns>
        NewUser AddUser(NewUser user, CloudIdentity identity = null);

        /// <summary>
        /// Updates the details for the specified user.
        /// </summary>
        /// <param name="user">The <see cref="User"/> details to update.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="User"/>.</returns>
        User UpdateUser(User user, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified user from the account.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteUser(string userId, CloudIdentity identity = null);

        /// <summary>
        /// Lists the credentials for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="UserCredential"/></returns>
        IEnumerable<UserCredential> ListUserCredentials(string userId, CloudIdentity identity = null);

        /// <summary>
        /// Lists the tenants for the user.
        /// </summary>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Tenant"/></returns>
        IEnumerable<Tenant> ListTenants(CloudIdentity identity = null);

        /// <summary>
        /// Gets the user access details.
        /// </summary>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <param name="forceCacheRefresh">If set to <c>true</c> the cache will be bypassed on retrieval.</param>
        /// <returns>The <see cref="UserAccess"/> details</returns>
        UserAccess GetUserAccess(CloudIdentity identity = null, bool forceCacheRefresh = false);

        /// <summary>
        /// Gets the specified user credential.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="credentialKey">The credential key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="UserCredential"/> details</returns>
        UserCredential GetUserCredential(string userId, string credentialKey, CloudIdentity identity = null);
    }
}