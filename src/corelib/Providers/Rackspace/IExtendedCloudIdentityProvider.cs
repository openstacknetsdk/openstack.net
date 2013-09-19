using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;

namespace net.openstack.Providers.Rackspace
{
    public interface IExtendedCloudIdentityProvider : IIdentityProvider
    {
        /// <summary>
        /// Lists all roles.
        /// </summary>
        /// <param name="serviceId">The ID of service to filter by.</param>
        /// <param name="markerId">The index of the last item in the previous list. <remarks>Used for pagination.</remarks></param>
        /// <param name="limit">Indicates the number of items to return <remarks>Default value depends on the provider's implemenation.</remarks><remarks>Used for pagination.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Role"/></returns>
        IEnumerable<Role> ListRoles(string serviceId = null, string markerId = null, int? limit = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists all users by role.
        /// </summary>
        /// <param name="roleId">The role ID. The behavior is unspecified if this is not obtained from <see cref="Role.Id"/>.</param>
        /// <param name="enabled">Allows you to filter enabled or un-enabled users. If the value is <c>null</c>, a provider-specific default value is used.</param>
        /// <param name="marker">The index of the last item in the previous list. Used for pagination. If the value is <c>null</c>, the list starts at the beginning.</param>
        /// <param name="limit">Indicates the maximum number of items to return. Used for pagination. If the value is <c>null</c>, a provider-specific default value is used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of <see cref="Role"/> objects describing the requested roles.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than 0 or greater than 1000.</exception>
        /// <exception cref="NotSupportedException">If the provider does not support the given <paramref name="identity"/> type.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        IEnumerable<User> ListUsersByRole(string roleId, bool? enabled = null, int? marker = null, int? limit = null, CloudIdentity identity = null);

        /// <summary>
        /// Create a new role.
        /// </summary>
        /// <param name="name">The name for the new role.</param>
        /// <param name="description">The description for the new role.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="Role"/></returns>
        Role AddRole(string name, string description, CloudIdentity identity = null);

        /// <summary>
        /// Retrieves the specified role
        /// </summary>
        /// <param name="roleId">The role ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="Role"/></returns>
        Role GetRole(string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Adds the specified role to the user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleId">The role ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool AddRoleToUser(string userId, string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified role from the user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="roleId">The role ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user credentials.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="apiKey">The new API key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns></returns>
        UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified user credentials.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteUserCredentials(string userId, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user password.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetUserPassword(string userId, string password, CloudIdentity identity = null);

        /// <summary>
        /// Sets the specified user password.
        /// </summary>
        /// <param name="user">The <see cref="User"/>.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetUserPassword(User user, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetUserPassword(string userId, string username, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user credentials.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="UserCredential"/></returns>
        UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user credentials.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="username">The new username.</param>
        /// <param name="apiKey">The new API key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="UserCredential"/></returns>
        UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity = null);
    }
}