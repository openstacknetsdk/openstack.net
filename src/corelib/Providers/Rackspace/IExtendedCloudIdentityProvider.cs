using System.Collections.Generic;
using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public interface IExtendedCloudIdentityProvider : ICloudIdentityProvider
    {
        /// <summary>
        /// Lists all roles.
        /// </summary>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="Role"/></returns>
        IEnumerable<Role> ListRoles(CloudIdentity identity = null);

        /// <summary>
        /// Create a new role.
        /// </summary>
        /// <param name="role">The new role.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="Role"/></returns>
        Role AddRole(Role role, CloudIdentity identity = null);

        /// <summary>
        /// Retrieves the specified role
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="Role"/></returns>
        Role GetRole(string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Adds the specified role to the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool AddRoleToUser(string userId, string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified role from user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user credentials.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="apiKey">The new API key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns></returns>
        UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified user credentials.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool DeleteUserCredentials(string userId, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user password.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetUserPassword(string userId, string password, CloudIdentity identity = null);

        /// <summary>
        /// Sets the specified user password.
        /// </summary>
        /// <param name="user">The <see cref="User"/>.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetUserPassword(User user, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        bool SetUserPassword(string userId, string username, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user credentials.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="UserCredential"/></returns>
        UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified user credentials.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="username">The new username.</param>
        /// <param name="apiKey">The new API key.</param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity" /><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>The <see cref="UserCredential"/></returns>
        UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity = null);
    }
}