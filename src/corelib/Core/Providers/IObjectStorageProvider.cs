﻿using System;
using System.Collections.Generic;
using System.IO;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Core.Providers
{
    /// <summary>
    /// Represents a provider for the OpenStack Object Storage service.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/">OpenStack Object Storage API v1 Reference</seealso>
    public interface IObjectStorageProvider
    {
        #region Container

        /// <summary>
        /// Gets a list of containers stored in the account.
        /// </summary>
        /// <param name="limit">The maximum number of containers to return. If the value is <c>null</c>, a provider-specific default is used.</param>
        /// <param name="marker">When specified, only containers with names greater than <paramref name="marker"/> are returned. If the value is <c>null</c>, the list starts at the beginning.</param>
        /// <param name="markerEnd">When specified, only containers with names less than <paramref name="markerEnd"/> are returned. If the value is <c>null</c>, the list proceeds to the end, or until the <paramref name="limit"/> is reached.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of <see cref="Container"/> objects containing the details of the specified containers.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than 0.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/s_listcontainers.html">List Containers (OpenStack Object Storage API v1 Reference)</seealso>
        IEnumerable<Container> ListContainers(int? limit = null, string marker = null, string markerEnd = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates a container if it does not already exist.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="headers">A collection of custom HTTP headers to associate with the container (see <see cref="GetContainerHeader"/>).</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>
        /// This method returns one of the following <see cref="ObjectStore"/> values.
        ///
        /// <list type="bullet">
        /// <item><see cref="ObjectStore.ContainerCreated"/> - if the container was created.</item>
        /// <item><see cref="ObjectStore.ContainerExists"/> - if the container was not created because it already exists.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-container.html">Create Container (OpenStack Object Storage API v1 Reference)</seealso>
        ObjectStore CreateContainer(string container, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes a container, and optionally all objects stored in the container.
        /// </summary>
        /// <remarks>
        /// Containers cannot be deleted unless they are empty. The <paramref name="deleteObjects"/> parameter provides
        /// a mechanism to combine the deletion of container objects with the deletion of the container itself.
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="deleteObjects">When <c>true</c>, all objects in the specified container are deleted before deleting the container.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ContainerNotEmptyException">If the container could not be deleted because it was not empty and <paramref name="deleteObjects"/> was <c>false</c>.</exception>
        /// <exception cref="ItemNotFoundException">If the specified container does not exist.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/delete-container.html">Delete Container (OpenStack Object Storage API v1 Reference)</seealso>
        void DeleteContainer(string container, bool deleteObjects = false, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the non-metadata headers associated with the container.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of non-metadata HTTP headers returned with the container.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-container-metadata.html">Get Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the container metadata.
        /// </summary>
        /// <remarks>
        /// The metadata associated with containers in the Object Storage Service are
        /// case-insensitive.
        ///
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of metadata associated with the container.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-container-metadata.html">Get Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the container CDN header.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A <see cref="ContainerCDN"/> object describing the CDN properties of the container.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
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
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN_Container_Details-d1e2566.html">View CDN Container Details (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        ContainerCDN GetContainerCDNHeader(string container, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Gets a list of CDN properties for a group of containers.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <param name="limit">The maximum number of containers to return. If the value is <c>null</c>, a provider-specific default is used.</param>
        /// <param name="markerId">When specified, only containers with names greater than <paramref name="markerId"/> are returned. If the value is <c>null</c>, the list starts at the beginning.</param>
        /// <param name="markerEnd">When specified, only containers with names less than <paramref name="markerEnd"/> are returned. If the value is <c>null</c>, the list proceeds to the end, or until the <paramref name="limit"/> is reached.</param>
        /// <param name="cdnEnabled">If set to <c>true</c>, the result is filtered to only include CDN-enabled containers.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of <see cref="ContainerCDN"/> objects describing the CDN properties of the specified containers.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than 0.</exception>
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
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/List_CDN-Enabled_Containers-d1e2414.html">List CDN-Enabled Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string markerId = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null);

        /// <overloads>
        /// <summary>
        /// When you CDN-enable a container, all the objects within it become available through the
        /// Content Delivery Network (CDN). Similarly, once a container is CDN-enabled, any objects
        /// added to it in the storage service become CDN-enabled.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This feature is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        /// </overloads>
        ///
        /// <summary>
        /// Enables CDN on the container using the specified TTL and without log retention.
        /// </summary>
        /// <remarks>
        /// If the specified container is already CDN-enabled, this method updates the TTL
        /// for the container based on the <paramref name="timeToLive"/> argument.
        ///
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        ///
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="timeToLive">The time (in seconds) to cache objects in the CDN. Each time the object is accessed after the TTL expires, the CDN re-fetches and caches the object for the TTL period.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of HTTP headers included in the response to the REST request.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="timeToLive"/> is less than 0.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="TTLLengthException">If the provider does not support the specified <paramref name="timeToLive"/>.</exception>
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
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        Dictionary<string, string> EnableCDNOnContainer(string container, long timeToLive, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables CDN on the container using the specified log retention and a provider-specific
        /// default TTL.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="logRetention"><c>true</c> to enable log retention on the container; otherwise, <c>false</c>.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <returns>A collection of HTTP headers included in the response to the REST request.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
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
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables CDN on the container using the specified TTL and log retention values.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container.</param>
        /// <param name="timeToLive">The time (in seconds) to cache objects in the CDN. Each time the object is accessed after the TTL expires, the CDN re-fetches and caches the object for the TTL period.</param>
        /// <param name="logRetention"><c>true</c> to enable log retention on the container; otherwise, <c>false</c>.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of HTTP headers included in the response to the REST request.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="timeToLive"/> is less than 0.</exception>
        /// <exception cref="TTLLengthException">If the provider does not support the specified <paramref name="timeToLive"/>.</exception>
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
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        Dictionary<string, string> EnableCDNOnContainer(string container, long timeToLive, bool logRetention, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Disables CDN on the container.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of HTTP headers included in the response to the REST request.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
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
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enable_a_Container-d1e2665.html">CDN-Enable a Container (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Updates the metadata associated with the container. This method is used to add, update, and
        /// remove metadata items associated with a storage container.
        /// </summary>
        /// <remarks>
        /// Each key/value pair in <paramref name="metadata"/> represents an updated metadata item.
        /// If the value is <c>null</c> or empty, then the metadata item represented by the key is
        /// removed if it exists. If a metadata item already exists for the key, its value is updated.
        /// Otherwise, a new metadata item is added for the key/value pair.
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="metadata">The account metadata to update.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains a key or value with invalid characters.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains a key that is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains a key or value with characters that are not supported by the implementation.</para>
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Update_Container_Metadata-d1e1900.html">Create or Update Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void UpdateContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes multiple metadata items from the container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="keys">The metadata items to delete.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> contains any <c>null</c> or empty values.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> contains a key with characters that are not supported by the implementation.</para>
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/delete-container-metadata.html">Delete Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void DeleteContainerMetadata(string container, IEnumerable<string> keys, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified metadata item from the container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="key">The metadata item to delete.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="key"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="key"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>If <paramref name="key"/> contains a character that is not supported by the implementation.</para>
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/delete-container-metadata.html">Delete Container Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void DeleteContainerMetadata(string container, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Sets the CDN headers for the specified container, replacing any existing headers.
        /// </summary>
        /// <remarks>
        /// <note type="warning">
        /// This method replaces <em>all</em> existing CDN headers for the container with the
        /// values found in <paramref name="headers"/>.
        /// </note>
        ///
        /// <note type="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="headers">The complete set of CDN headers for the container.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
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
        /// <exception cref="ItemNotFoundException">If the specified container does not exist.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN_Container_Services-d1e2632.html">CDN Container Services (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        void UpdateContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables anonymous web access to the static content of the specified container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">The index file to serve when users browse the container, such as <fictionalUri>index.html</fictionalUri>. This is the value for the <see cref="CloudFilesProvider.WebIndex"/> header.</param>
        /// <param name="error">The suffix for the file to serve when an error occurs. If the value is <fictionalUri>error.html</fictionalUri> and a 404 (not found) error occurs, the file <fictionalUri>400error.html</fictionalUri> will be served to the user. This is the value for the <see cref="CloudFilesProvider.WebError"/> header.</param>
        /// <param name="css">The style sheet to use for file listings, such as <fictionalUri>lists.css</fictionalUri>. This is the value for the <see cref="CloudFilesProvider.WebListingsCSS"/> header.</param>
        /// <param name="listing"><c>true</c> to allow users to browse a list of files in the container when no index file is available; otherwise <c>false</c>. This is the value for the <see cref="CloudFilesProvider.WebListings"/> header.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="index"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="css"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="index"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="css"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">
        /// If <paramref name="index"/> is not a valid object name.
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is not a valid object name.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="css"/> is not a valid object name.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="CDNNotEnabledException">If the provider requires containers be CDN-enabled before they can be accessed from the web, and the <see cref="ContainerCDN.CDNEnabled"/> property is false.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference)</seealso>
        void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Enables anonymous web access to the static content of the specified container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">The index file to serve when users browse the container, such as <fictionalUri>index.html</fictionalUri>. This is the value for the <see cref="CloudFilesProvider.WebIndex"/> header.</param>
        /// <param name="error">The suffix for the file to serve when an error occurs. If the value is <fictionalUri>error.html</fictionalUri> and a 404 (not found) error occurs, the file <fictionalUri>400error.html</fictionalUri> will be served to the user. This is the value for the <see cref="CloudFilesProvider.WebError"/> header.</param>
        /// <param name="listing"><c>true</c> to allow users to browse a list of files in the container when no index file is available; otherwise <c>false</c>. This is the value for the <see cref="CloudFilesProvider.WebListings"/> header.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="index"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="index"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">
        /// If <paramref name="index"/> is not a valid object name.
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is not a valid object name.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="CDNNotEnabledException">If the provider requires containers be CDN-enabled before they can be accessed from the web, and the <see cref="ContainerCDN.CDNEnabled"/> property is false.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference)</seealso>
        void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Enables anonymous web access to the static content of the specified container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="css">The style sheet to use for file listings, such as <fictionalUri>lists.css</fictionalUri>. This is the value for the <see cref="CloudFilesProvider.WebListingsCSS"/> header.</param>
        /// <param name="listing"><c>true</c> to allow users to browse a list of files in the container when no index file is available; otherwise <c>false</c>. This is the value for the <see cref="CloudFilesProvider.WebListings"/> header.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="css"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="css"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="css"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="CDNNotEnabledException">If the provider requires containers be CDN-enabled before they can be accessed from the web, and the <see cref="ContainerCDN.CDNEnabled"/> property is false.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference)</seealso>
        void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Enables anonymous web access to the static content of the specified container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">The index file to serve when users browse the container, such as <fictionalUri>index.html</fictionalUri>. This is the value for the <see cref="CloudFilesProvider.WebIndex"/> header.</param>
        /// <param name="error">The suffix for the file to serve when an error occurs. If the value is <fictionalUri>error.html</fictionalUri> and a 404 (not found) error occurs, the file <fictionalUri>400error.html</fictionalUri> will be served to the user. This is the value for the <see cref="CloudFilesProvider.WebError"/> header.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="index"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="index"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">
        /// If <paramref name="index"/> is not a valid object name.
        /// <para>-or-</para>
        /// <para>If <paramref name="error"/> is not a valid object name.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="CDNNotEnabledException">If the provider requires containers be CDN-enabled before they can be accessed from the web, and the <see cref="ContainerCDN.CDNEnabled"/> property is false.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference)</seealso>
        void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Disables anonymous web access to the static content of the specified container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="CDNNotEnabledException">If the provider requires containers be CDN-enabled before they can be accessed from the web, and the <see cref="ContainerCDN.CDNEnabled"/> property is false.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/Create_Static_Website-dle4000.html">Create Static Website (OpenStack Object Storage API v1 Reference)</seealso>
        void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        #endregion

        #region Container Objects

        /// <summary>
        /// Gets the non-metadata headers for the specified object.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of non-metadata headers associated with the specified object.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-object-metadata.html">Get Object Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        Dictionary<string, string> GetObjectHeaders(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <remarks>
        /// The metadata associated with objects in the Object Storage Service are
        /// case-insensitive.
        ///
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of metadata associated with the object.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-object-metadata.html">Get Object Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Sets the object metadata, replacing any existing metadata values.
        /// </summary>
        /// <remarks>
        /// <note type="warning">
        /// This method replaces all existing metadata for the object with the values
        /// found in <paramref name="metadata"/>. To add or change existing metadata values
        /// without affecting all metadata for the object, first call <see cref="GetObjectMetaData"/>,
        /// modify the returned <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>,
        /// then call <see cref="UpdateObjectMetadata"/> with the modified metadata dictionary.
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name.</param>
        /// <param name="metadata">The complete metadata to associate with the object.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/update-object-metadata.html">Update Object Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void UpdateObjectMetadata(string container, string objectName, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes multiple metadata items from the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name.</param>
        /// <param name="keys">The metadata items to delete.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> contains any <c>null</c> or empty values.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/update-object-metadata.html">Update Object Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void DeleteObjectMetadata(string container, string objectName, IEnumerable<string> keys, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the specified metadata item from the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name.</param>
        /// <param name="key">The metadata item to delete.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="key"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="key"/> is empty.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/update-object-metadata.html">Update Object Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void DeleteObjectMetadata(string container, string objectName, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Lists the objects in a container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="limit">The maximum number of objects to return. If the value is <c>null</c>, a provider-specific default is used.</param>
        /// <param name="marker">When specified, only objects with names greater than <paramref name="marker"/> are returned. If the value is <c>null</c>, the list starts at the beginning.</param>
        /// <param name="markerEnd">When specified, only objects with names less than <paramref name="markerEnd"/> are returned. If the value is <c>null</c>, the list proceeds to the end, or until the <paramref name="limit"/> is reached.</param>
        /// <param name="prefix">Prefix of object names to include</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of <see cref="ContainerObject"/> objects containing the details of the specified objects.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="container"/> is empty.</exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than 0.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/list-objects.html">List Objects (OpenStack Object Storage API v1 Reference)</seealso>
        IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string marker = null, string markerEnd = null, string prefix = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="filePath">The source file path. Example <localUri>c:\folder1\folder2\image_name.jpeg</localUri></param>
        /// <param name="objectName">The destination object name. If <c>null</c>, the filename portion of <paramref name="filePath"/> will be used.</param>
        /// <param name="contentType">The content type of the created object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="chunkSize">The buffer size to use for copying streaming data.</param>
        /// <param name="headers">A collection of custom HTTP headers to associate with the object (see <see cref="GetObjectHeaders"/>).</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="progressUpdated">A callback for progress updates. If the value is <c>null</c>, no progress updates are reported.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="filePath"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="filePath"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkSize"/> is less than 0.</exception>
        /// <exception cref="FileNotFoundException">If the file <paramref name="filePath"/> could not be found.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-object.html">Create or Update Object (OpenStack Object Storage API v1 Reference)</seealso>
        void CreateObjectFromFile(string container, string filePath, string objectName = null, string contentType = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates an object using data from a <see cref="Stream"/>. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="stream">A <see cref="Stream"/> providing the data for the file.</param>
        /// <param name="objectName">The destination object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="contentType">The content type of the created object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="chunkSize">The buffer size to use for copying streaming data.</param>
        /// <param name="headers">A collection of custom HTTP headers to associate with the object (see <see cref="GetObjectHeaders"/>).</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="progressUpdated">A callback for progress updates. If the value is <c>null</c>, no progress updates are reported.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkSize"/> is less than 0.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-object.html">Create or Update Object (OpenStack Object Storage API v1 Reference)</seealso>
        void CreateObject(string container, Stream stream, string objectName, string contentType = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets an object, writing the data to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The source object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="chunkSize">The buffer size to use for copying streaming data.</param>
        /// <param name="headers">A collection of custom HTTP headers to include with the request.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="verifyEtag">If <c>true</c> and the object includes an ETag, the retrieved data will be verified before returning.</param>
        /// <param name="progressUpdated">A callback for progress updates. If the value is <c>null</c>, no progress updates are reported.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="outputStream"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkSize"/> is less than 0.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="InvalidETagException">If <paramref name="verifyEtag"/> is <c>true</c>, the object includes an ETag header, and the downloaded data does not match the ETag header value.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-object.html">Get Object Details (OpenStack Object Storage API v1 Reference)</seealso>
        void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets an object, saving the data to the specified file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="saveDirectory">The destination directory name. Example <localUri>c:\user\</localUri></param>
        /// <param name="objectName">The source object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="fileName">The destination file name. Example <localUri>image_name1.jpeg</localUri></param>
        /// <param name="chunkSize">The buffer size to use for copying streaming data.</param>
        /// <param name="headers">A collection of custom HTTP headers to include with the request.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="verifyEtag">If <c>true</c> and the object includes an ETag, the retrieved data will be verified before returning.</param>
        /// <param name="progressUpdated">A callback for progress updates. If the value is <c>null</c>, no progress updates are reported.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="saveDirectory"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="saveDirectory"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="chunkSize"/> is less than 0.</exception>
        /// <exception cref="DirectoryNotFoundException">If the directory <paramref name="saveDirectory"/> could not be found.</exception>
        /// <exception cref="IOException">If an error occurs while writing to the destination file.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="InvalidETagException">If <paramref name="verifyEtag"/> is <c>true</c>, the object includes an ETag header, and the downloaded data does not match the ETag header value.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-object.html">Get Object Details (OpenStack Object Storage API v1 Reference)</seealso>
        void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Copies an object to a new location within the Object Storage provider.
        /// </summary>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObjectName">The source object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObjectName">The destination object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="destinationContentType">The content type of the destination object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="headers">A collection of custom HTTP headers to associate with the created object (see <see cref="GetObjectHeaders"/>).</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="sourceContainer"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObjectName"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObjectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="sourceContainer"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObjectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObjectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">
        /// If <paramref name="sourceContainer"/> is not a valid container name.
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is not a valid container name.</para>
        /// </exception>
        /// <exception cref="ObjectNameException">
        /// If <paramref name="sourceObjectName"/> is not a valid object name.
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObjectName"/> is not a valid object name.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/copy-object.html">Copy Object (OpenStack Object Storage API v1 Reference)</seealso>
        void CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, string destinationContentType = null, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Moves an object to a new location within the Object Storage provider.
        /// </summary>
        /// <remarks>
        /// The original object is removed only if the move is completed successfully.
        ///
        /// <note type="implement">
        /// If your specific provider does not provide a "Move Object" API function, this
        /// method may be implemented by performing a <see cref="CopyObject"/> operation,
        /// followed by a <see cref="DeleteObject"/> operation if the copy completed
        /// successfully.
        /// </note>
        /// </remarks>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObjectName">Name of the source object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObjectName">Name of the destination object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContentType">The content type of the destination object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="headers">A collection of custom HTTP headers to associate with the object (see <see cref="GetObjectHeaders"/>).</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="sourceContainer"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObjectName"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObjectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="sourceContainer"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObjectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObjectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">
        /// If <paramref name="sourceContainer"/> is not a valid container name.
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is not a valid container name.</para>
        /// </exception>
        /// <exception cref="ObjectNameException">
        /// If <paramref name="sourceObjectName"/> is not a valid object name.
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObjectName"/> is not a valid object name.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/copy-object.html">Copy Object (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/delete-object.html">Delete Object (OpenStack Object Storage API v1 Reference)</seealso>
        void MoveObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, string destinationContentType = null, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes an object from a container.
        /// </summary>
        /// <remarks>
        /// To support large files, the object storage services allows for a single logical file
        /// to be split into multiple segments. The <paramref name="deleteSegments"/> parameter
        /// provides a way to delete a segmented file as though it were stored as a single object
        /// by deleting both the logical file's metadata and the individual segments. The
        /// <paramref name="deleteSegments"/> parameter is ignored if the specified object is not
        /// a segmented file.
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="headers">A collection of custom HTTP headers to include with the request.</param>
        /// <param name="deleteSegments">Indicates whether the file's segments should be deleted if any exist.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/large-object-creation.html">Create Large Objects (OpenStack Object Storage API v1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/delete-object.html">Delete Object (OpenStack Object Storage API v1 Reference)</seealso>
        void DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, bool deleteSegments = true, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Purges an object from the CDN, sending an email notification to the specified addresses when the object has been purged.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">The object name. Example <localUri>image_name.jpeg</localUri></param>
        /// <param name="emails">The email addresses to notify once the object has been purged. If this value is <c>null</c>, no email notifications are sent.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="container"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="objectName"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="emails"/> contains a null or empty value.</para>
        /// </exception>
        /// <exception cref="ContainerNameException">If <paramref name="container"/> is not a valid container name.</exception>
        /// <exception cref="ObjectNameException">If <paramref name="objectName"/> is not a valid object name.</exception>
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
        /// <exception cref="CDNNotEnabledException">If the specified <paramref name="container"/> is not CDN-enabled.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Purge_CDN-Enabled_Objects-d1e3858.html">Purge CDN-Enabled Objects (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        void PurgeObjectFromCDN(string container, string objectName, IEnumerable<string> emails = null, string region = null, CloudIdentity identity = null);

        #endregion

        #region Accounts

        /// <summary>
        /// Gets the non-metadata headers associated with the specified account.
        /// </summary>
        /// <remarks>
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of non-metadata headers associated with the account.</returns>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-account-metadata.html">Get Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the account metadata.
        /// </summary>
        /// <remarks>
        /// The metadata associated with accounts in the Object Storage Service are
        /// case-insensitive.
        ///
        /// <note type="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </note>
        /// </remarks>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <returns>A collection of metadata associated with the account.</returns>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// <para>-or-</para>
        /// <para><paramref name="useInternalUrl"/> is <c>true</c> and the provider does not support internal URLs.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/retrieve-account-metadata.html">Get Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Updates the metadata associated with the account. This method is used to add, update, and
        /// remove metadata items associated with a storage account.
        /// </summary>
        /// <remarks>
        /// Each key/value pair in <paramref name="metadata"/> represents an updated metadata item.
        /// If the value is <c>null</c> or empty, then the metadata item represented by the key is
        /// removed if it exists. If a metadata item already exists for the key, its value is updated.
        /// Otherwise, a new metadata item is added for the key/value pair.
        /// </remarks>
        /// <param name="metadata">The account metadata to update.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="metadata"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="metadata"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains a key or value with invalid characters.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains a key that is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the provider does not support the given <paramref name="identity"/> type.
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains a key or value with characters that are not supported by the implementation.</para>
        /// <para>-or-</para>
        /// <para>The specified <paramref name="region"/> is not supported.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="identity"/> is <c>null</c> and no default identity is available for the provider.
        /// <para>-or-</para>
        /// <para>If <paramref name="region"/> is <c>null</c> and no default region is available for the provider.</para>
        /// </exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-account-metadata.html">Create or Update Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        #endregion
    }
}
