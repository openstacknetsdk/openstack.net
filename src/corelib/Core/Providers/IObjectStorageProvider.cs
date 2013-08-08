using System;
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
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/">Object Storage API v1 Reference</seealso>
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
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-container.html">Create Container (OpenStack Object Storage API v1 Reference)</seealso>
        ObjectStore CreateContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

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
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
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
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
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
        /// <alert note="note">
        /// This feature is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
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
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
        ///
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// Sets the container metadata, replacing any existing metadata values.
        /// </summary>
        /// <remarks>
        /// <alert class="warning">
        /// This method replaces all existing metadata for the container with the values
        /// found in <paramref name="metadata"/>. To add or change existing metadata values
        /// without affecting all metadata for the container, first call <see cref="GetContainerMetaData"/>,
        /// modify the returned <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>,
        /// then call <see cref="UpdateContainerMetadata"/> with the modified metadata dictionary.
        /// </alert>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="metadata">The complete metadata to associate with the container.</param>
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
        /// Adds the non-metadata headers to the specified container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> is <c>null</c>.</para>
        /// </exception>
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
        /// <exception cref="ItemNotFoundException">If the specified container does not exist.</exception>
        /// <exception cref="ResponseException">If the REST API request failed.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-container.html">Create Container (OpenStack Object Storage API v1 Reference)</seealso>
        void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Sets the CDN headers for the specified container, replacing any existing headers.
        /// </summary>
        /// <remarks>
        /// <alert class="warning">
        /// This method replaces <em>all</em> existing CDN headers for the container with the
        /// values found in <paramref name="headers"/>.
        /// </alert>
        ///
        /// <alert note="note">
        /// This method is a Rackspace-specific extension to the OpenStack Object Storage Service.
        /// </alert>
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
        /// Gets the object headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        Dictionary<string, string> GetObjectHeaders(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object meta data.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of Meta data</returns>
        Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Updates the specified metadata items for the object.  <remarks>If the metadata key already exists, it will updated, else it will be added.</remarks>
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="metadata">Dictionary of metadata keys/value pairs to associate with the object</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of Meta data</returns>
        void UpdateObjectMetadata(string container, string objectName, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes multiple metadata keys, values from an object
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="metadata">Dictionary of metadata keys, values to delete</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void DeleteObjectMetadata(string container, string objectName, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes a single metadata key, value from an object
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="key">Single metadata key to delete</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void DeleteObjectMetadata(string container, string objectName, string key, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);


        /// <summary>
        /// Lists the objects.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="marker">The marker.</param>
        /// <param name="markerEnd">The marker end.</param>
        /// <param name="prefix">Prefix of object names to include</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.ContainerObject"/></returns>
        IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string marker = null, string markerEnd = null, string prefix = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates the object from file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="filePath">The file path.<remarks>Example c:\folder1\folder2\image_name.jpeg</remarks></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg.  If null, the name of the upload file will be used</remarks></param>
        /// <param name="contentType">The content type of the created object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void CreateObjectFromFile(string container, string filePath, string objectName = null, string contentType = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="stream">The stream. <see cref="Stream"/></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="contentType">The content type of the created object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void CreateObject(string container, Stream stream, string objectName, string contentType = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="outputStream">The output stream.<see cref="Stream"/></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="verifyEtag">If set to <c>true</c> will verify etag.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="InvalidETagException"></exception>
        void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object save to file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="saveDirectory">The save directory path. <remarks>Example c:\user\</remarks></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="fileName">Name of the file.<remarks>Example image_name1.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="verifyEtag">If set to <c>true</c> will verify etag.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Copies the object.
        /// </summary>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObjectName">Name of the source object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObjectName">Name of the destination object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContentType">The content type of the destination object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="headers">A list of HTTP headers to send to the service. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, string destinationContentType = null, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Moves an object.  <remarks>The original source object will be deleted only if the move is successful.</remarks>
        /// </summary>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObjectName">Name of the source object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObjectName">Name of the destination object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContentType">The content type of the destination object. If the value is <c>null</c> or empty, the content type of the created object is unspecified.</param>
        /// <param name="headers">A list of HTTP headers to send to the service. </param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        ///<param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore MoveObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, string destinationContentType = null, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes a specified object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="headers">A list of HTTP headers to send to the service. </param>
        /// <param name="deleteSegments">Indicates whether the file's segments should be deleted if any exist.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="useInternalUrl">If set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, bool deleteSegments = true, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="CDNNotEnabledException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="email">Email Address.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="CDNNotEnabledException"></exception>
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string email, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="emails">string[] of email address.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="CDNNotEnabledException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string[] emails, string region = null, CloudIdentity identity = null);
        #endregion

        #region Accounts

        /// <summary>
        /// Gets the non-metadata headers associated with the specified account.
        /// </summary>
        /// <remarks>
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// <alert class="implement">
        /// The resulting <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>
        /// should use the <see cref="StringComparer.OrdinalIgnoreCase"/> equality comparer to ensure
        /// lookups are not case sensitive.
        /// </alert>
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
        /// Sets the account metadata, replacing any existing metadata values.
        /// </summary>
        /// <remarks>
        /// <alert class="warning">
        /// This method replaces all existing metadata for the account with the values
        /// found in <paramref name="metadata"/>. To add or change existing metadata values
        /// without affecting all metadata for the account, first call <see cref="GetAccountMetaData"/>,
        /// modify the returned <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string&gt;</see>,
        /// then call <see cref="UpdateAccountMetadata"/> with the modified metadata dictionary.
        /// </alert>
        /// </remarks>
        /// <param name="metadata">The complete metadata to associate with the account.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="metadata"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="metadata"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</exception>
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
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-account-metadata.html">Create or Update Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Sets the non-metadata headers associated with the specified account, replacing any existing non-metadata headers.
        /// </summary>
        /// <param name="headers">The complete set of headers to associate with the account.</param>
        /// <param name="region">The region in which to execute this action. If not specified, the user's default region will be used.</param>
        /// <param name="useInternalUrl"><c>true</c> to use the endpoint's <see cref="Endpoint.InternalURL"/>; otherwise <c>false</c> to use the endpoint's <see cref="Endpoint.PublicURL"/>.</param>
        /// <param name="identity">The cloud identity to use for this request. If not specified, the default identity for the current provider instance will be used.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="headers"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="headers"/> contains two equivalent keys when compared using <see cref="StringComparer.OrdinalIgnoreCase"/>.</exception>
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
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/create-update-account-metadata.html">Create or Update Account Metadata (OpenStack Object Storage API v1 Reference)</seealso>
        void UpdateAccountHeaders(Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        #endregion
    }
}
