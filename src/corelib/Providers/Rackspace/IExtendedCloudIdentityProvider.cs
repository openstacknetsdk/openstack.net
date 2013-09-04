﻿using System;
using System.Collections.Generic;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Represents an identity provider that implements Rackspace-specific extensions to the
    /// OpenStack Identity API.
    /// </summary>
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
        /// Create a new role.
        /// </summary>
        /// <param name="name">The name for the new role.</param>
        /// <param name="description">The description for the new role.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="Role"/> containing the details of the added role.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_addRole_v2.0_OS-KSADM_roles_Role_Operations_OS-KSADM.html">Add Role (OpenStack Identity Service API v2.0 Reference)</seealso>
        Role AddRole(string name, string description, CloudIdentity identity = null);

        /// <summary>
        /// Gets details about the specified role.
        /// </summary>
        /// <param name="roleId">The role ID. The behavior is unspecified if this is not obtained from <see cref="Role.Id">Role.Id</see>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="Role"/> containing the details of the role.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="roleId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="roleId"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/GET_getRoleByName_v2.0_OS-KSADM_roles_Role_Operations_OS-KSADM.html">Get Role by Name (OpenStack Identity Service API v2.0 Reference)</seealso>
        Role GetRole(string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Adds the specified global role to the user.
        /// </summary>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="roleId">The role ID. The behavior is unspecified if this is not obtained from <see cref="Role.Id"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the role was removed from the user; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="userId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="roleId"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="userId"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="roleId"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/PUT_addUserRole_v2.0_users__userId__roles_OS-KSADM__roleId__.html">Add Global Role to User (OpenStack Identity Service API v2.0 Reference)</seealso>
        bool AddRoleToUser(string userId, string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified global role from the user.
        /// </summary>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="roleId">The role ID. The behavior is unspecified if this is not obtained from <see cref="Role.Id"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the role was removed from the user; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="userId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="roleId"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="userId"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="roleId"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/DELETE_deleteUserRole_v2.0_users__userId__roles_OS-KSADM__roleId__.html">Delete Global Role from User (OpenStack Identity Service API v2.0 Reference)</seealso>
        bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity = null);

        /// <summary>
        /// Updates the API key for the specified user.
        /// </summary>
        /// <remarks>
        /// This method is a Rackspace-specific usage scenario for the
        /// <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials</see>
        /// method, where the credentials are always specified in the form of an API key.
        /// </remarks>
        /// <returns>A <see cref="UserCredential"/> object containing the updated user information.</returns>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="apiKey">The new API key.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="userId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="userId"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity = null);

        /// <summary>
        /// Deletes API key credentials for the specified user.
        /// </summary>
        /// <remarks>
        /// This method is a Rackspace-specific usage scenario for the
        /// <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/DELETE_deleteUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Delete User Credentials</see>
        /// method, where the credentials are always specified in the form of an API key.
        /// </remarks>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the credentials were removed from the user; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="userId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="userId"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/DELETE_deleteUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Delete User Credentials</seealso>
        bool DeleteUserCredentials(string userId, CloudIdentity identity = null);

        /// <summary>
        /// Sets the password for the specified user.
        /// </summary>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="userId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="userId"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        bool SetUserPassword(string userId, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the username and password for the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="user"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="password"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="user"/>.<see cref="User.Id"/> is <c>null</c> or empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="user"/>.<see cref="User.Username"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        bool SetUserPassword(User user, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the username and password for the specified user.
        /// </summary>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="username">The new username.</param>
        /// <param name="password">The new password.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="userId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="username"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="userId"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="username"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="password"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        bool SetUserPassword(string userId, string username, string password, CloudIdentity identity = null);

        /// <summary>
        /// Updates the username and API key for the specified user.
        /// </summary>
        /// <remarks>
        /// This method is a Rackspace-specific usage scenario for the
        /// <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials</see>
        /// method, where the credentials are always specified in the form of an API key.
        /// </remarks>
        /// <param name="user">The user.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="UserCredential"/> object containing the updated user information.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="user"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="apiKey"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="user"/>.<see cref="User.Id"/> is <c>null</c> or empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="user"/>.<see cref="User.Username"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity = null);

        /// <summary>
        /// Updates the username and API key for the specified user.
        /// </summary>
        /// <remarks>
        /// This method is a Rackspace-specific usage scenario for the
        /// <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials</see>
        /// method, where the credentials are always specified in the form of an API key.
        /// </remarks>
        /// <param name="userId">The user ID. The behavior is unspecified if this is not obtained from <see cref="NewUser.Id"/> or <see cref="User.Id"/>.</param>
        /// <param name="username">The new username.</param>
        /// <param name="apiKey">The new API key.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="UserCredential"/> object containing the updated user information.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="userId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="username"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="userId"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="username"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="apiKey"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the <see href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Admin_API_Service_Developer_Operations-d1e1357.html">OS-KSADM Admin Extension</see>.
        /// <para>-or-</para>
        /// <para>If the provider does not support the given <paramref name="identity"/> type.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/POST_updateUserCredential_v2.0_users__userId__OS-KSADM_credentials__credential-type__.html">Update User Credentials (OpenStack Identity Service API v2.0 Reference)</seealso>
        UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity = null);
    }
}
