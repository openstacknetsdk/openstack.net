﻿using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;

namespace net.openstack.Core.Providers
{
    /// <summary>
    /// Represents a provider for the OpenStack Networking service.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-network/2.0/content/">Networking API v2.0 Reference</seealso>
    public interface INetworksProvider
    {
        /// <summary>
        /// List the networks configured for the account.
        /// </summary>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A list of <see cref="CloudNetwork"/> objects describing the networks for the account.</returns>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-network/2.0/content/List_Networks.html">List Networks (OpenStack Networking API v2.0 Reference)</seealso>
        IEnumerable<CloudNetwork> ListNetworks(string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Create a network with the given IP block.
        /// </summary>
        /// <param name="cidr">The IP block from which to allocate the network. For example, <c>172.16.0.0/24</c> or <c>2001:DB8::/64</c>.</param>
        /// <param name="label">The name of the new network. For example, <c>my_new_network</c>.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="CloudNetwork"/> instance containing details for the newly created network.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="cidr"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="label"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="cidr"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="label"/> is empty.</para>
        /// </exception>
        /// <exception cref="CidrFormatException">If <paramref name="cidr"/> is not in the correct format.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-network/2.0/content/Create_Network.html">Create Network (OpenStack Networking API v2.0 Reference)</seealso>
        CloudNetwork CreateNetwork(string cidr, string label, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Retrieve details for the specified network.
        /// </summary>
        /// <param name="networkId">ID of the network to retrieve. The behavior is unspecified if this is not obtained from <see cref="CloudNetwork.Id"/>.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="CloudNetwork"/> instance containing the network details.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="networkId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="networkId"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-network/2.0/content/List_Networks_Detail.html">Show Network (OpenStack Networking API v2.0 Reference)</seealso>
        CloudNetwork ShowNetwork(string networkId, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified network. <remarks>You cannot delete an isolated network unless the network is not attached to any server.</remarks>
        /// </summary>
        /// <param name="networkId">ID of the network to delete. The behavior is unspecified if this is not obtained from <see cref="CloudNetwork.Id"/>.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns><c>true</c> if the network was successfully deleted; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="networkId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="networkId"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-network/2.0/content/Delete_Network.html">Delete Network (OpenStack Networking API v2.0 Reference)</seealso>
        bool DeleteNetwork(string networkId, string region = null, CloudIdentity identity = null);
    }
}
