namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides extension methods for working with object versioning in the Object Storage Service.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ObjectVersioningExtensions
    {
        /// <summary>
        /// Gets the name of the <c>X-Versions-Location</c> HTTP header used for specifying the name of the container
        /// where old versions of objects in a container get placed when the objects are updated.
        /// </summary>
        public static readonly string VersionsLocation = "X-Versions-Location";

        /// <summary>
        /// Gets the name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.
        /// </summary>
        /// <param name="metadata">A <see cref="ContainerMetadata"/> instance containing the metadata associated with a
        /// container.</param>
        /// <returns>
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if the specified <paramref name="metadata"/> does not contain a value for the
        /// <see cref="VersionsLocation"/> header.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        public static ContainerName GetVersionsLocation(this ContainerMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            string location;
            if (!metadata.Headers.TryGetValue(VersionsLocation, out location) || string.IsNullOrEmpty(location))
                return null;

            // first, URL-decode the value
            location = UriUtility.UriDecode(location);

            // then UTF-8 decode the value
            location = StorageMetadata.DecodeHeaderValue(location);

            // then return the result as a ContainerName
            return new ContainerName(location);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareCreateContainerAsync"/> to
        /// include the <see cref="VersionsLocation"/> header, which specifies the name of the container where old
        /// versions of objects in a versioned container get placed when the objects are updated.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>This call works for vendors which use an OpenStack-compatible implementation of Object Versioning,
        /// where the containing where versions are placed is specified by the <see cref="VersionsLocation"/> HTTP
        /// header. For maximum portability when working with vendors that implement equivalent functionality in another
        /// manner, use the <see cref="IObjectVersioningExtension.PrepareCreateVersionedContainerAsync"/> or
        /// <see cref="CreateVersionedContainerAsync"/> method instead.</para>
        /// </note>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="versionsLocation">
        /// The name of the container where old versions of objects in a versioned container get placed when the objects
        /// are updated.
        /// </param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="versionsLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="apiCall"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="versionsLocation"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static CreateContainerApiCall WithVersionsLocation(this CreateContainerApiCall apiCall, ContainerName versionsLocation)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            if (versionsLocation == null)
                throw new ArgumentNullException("versionsLocation");

            return apiCall.WithVersionsLocationImpl(versionsLocation);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareCreateContainerAsync"/> to
        /// include the <see cref="VersionsLocation"/> header, which specifies the name of the container where old
        /// versions of objects in a versioned container get placed when the objects are updated.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>This call works for vendors which use an OpenStack-compatible implementation of Object Versioning,
        /// where the containing where versions are placed is specified by the <see cref="VersionsLocation"/> HTTP
        /// header. For maximum portability when working with vendors that implement equivalent functionality in another
        /// manner, use the <see cref="IObjectVersioningExtension.PrepareCreateVersionedContainerAsync"/> or
        /// <see cref="CreateVersionedContainerAsync"/> method instead.</para>
        /// </note>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare a
        /// <see cref="CreateContainerApiCall"/> HTTP API call.</param>
        /// <param name="versionsLocation">
        /// The name of the container where old versions of objects in a versioned container get placed when the objects
        /// are updated.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="versionsLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="versionsLocation"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<CreateContainerApiCall> WithVersionsLocation(this Task<CreateContainerApiCall> task, ContainerName versionsLocation)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (versionsLocation == null)
                throw new ArgumentNullException("versionsLocation");

            return task.WithVersionsLocationImpl(versionsLocation);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareUpdateContainerMetadataAsync"/>
        /// to include the <see cref="VersionsLocation"/> header, which specifies the name of the container where old
        /// versions of objects in a versioned container get placed when the objects are updated.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>This call works for vendors which use an OpenStack-compatible implementation of Object Versioning,
        /// where the containing where versions are placed is specified by the <see cref="VersionsLocation"/> HTTP
        /// header. For maximum portability when working with vendors that implement equivalent functionality in another
        /// manner, use the <see cref="IObjectVersioningExtension.PrepareSetVersionsLocationAsync"/> or
        /// <see cref="SetVersionsLocationAsync"/> method instead.</para>
        /// </note>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="versionsLocation">
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to clear the value for the <see cref="VersionsLocation"/> header associated
        /// with a container.</para>
        /// </param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="versionsLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static UpdateContainerMetadataApiCall WithVersionsLocation(this UpdateContainerMetadataApiCall apiCall, ContainerName versionsLocation)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            // allow location to be null as a way to remove the X-Versions-Location header

            return apiCall.WithVersionsLocationImpl(versionsLocation);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareUpdateContainerMetadataAsync"/>
        /// to include the <see cref="VersionsLocation"/> header, which specifies the name of the container where old
        /// versions of objects in a versioned container get placed when the objects are updated.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>This call works for vendors which use an OpenStack-compatible implementation of Object Versioning,
        /// where the containing where versions are placed is specified by the <see cref="VersionsLocation"/> HTTP
        /// header. For maximum portability when working with vendors that implement equivalent functionality in another
        /// manner, use the <see cref="IObjectVersioningExtension.PrepareSetVersionsLocationAsync"/> or
        /// <see cref="SetVersionsLocationAsync"/> method instead.</para>
        /// </note>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare a
        /// <see cref="UpdateContainerMetadataApiCall"/> HTTP API call.</param>
        /// <param name="versionsLocation">
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to clear the value for the <see cref="VersionsLocation"/> header associated
        /// with a container.</para>
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="versionsLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<UpdateContainerMetadataApiCall> WithVersionsLocation(this Task<UpdateContainerMetadataApiCall> task, ContainerName versionsLocation)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            // allow location to be null as a way to remove the X-Versions-Location header

            return task.WithVersionsLocationImpl(versionsLocation);
        }

        /// <summary>
        /// Create a versioned container in the Object Storage Service, which places a copy of old versions of objects
        /// in a separate container whenever the objects are updated.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="versionsLocation">
        /// The name of the container where old versions of objects in a versioned container get placed when the objects
        /// are updated.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="versionsLocation"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="CreateVersionedContainerApiCall"/>
        /// <seealso cref="IObjectVersioningExtension.PrepareCreateVersionedContainerAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task CreateVersionedContainerAsync(this IObjectStorageService service, ContainerName container, ContainerName versionsLocation, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IObjectVersioningExtension extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ObjectVersioning);
            return TaskBlocks.Using(
                () => extension.PrepareCreateVersionedContainerAsync(container, versionsLocation, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Get the name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.
        /// </summary>
        /// <remarks>
        /// <note type="caller">
        /// <para>This method supports the implementation of the object versioning feature for vendors which do not use
        /// the standard <see cref="VersionsLocation"/> HTTP header for specifying and enabling object versioning for a
        /// container. For OpenStack-compatible vendors, the <see cref="GetVersionsLocation"/> method provides more
        /// efficient access to the versions location when the <see cref="ContainerMetadata"/> is already
        /// available.</para>
        /// </note>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property contains the name of the container where old versions of objects
        /// in a versioned container get placed when the objects are updated, or <see langword="null"/> if object
        /// versioning is not enabled for the container.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetVersionsLocationApiCall"/>
        /// <seealso cref="ObjectVersioningExtensions.GetVersionsLocation"/>
        /// <seealso cref="IObjectVersioningExtension.PrepareGetVersionsLocationAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ContainerName> GetVersionsLocationAsync(this IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IObjectVersioningExtension extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ObjectVersioning);
            return TaskBlocks.Using(
                () => extension.PrepareGetVersionsLocationAsync(container, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Set the name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The name of the container to update.</param>
        /// <param name="versionsLocation">
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to disable object versioning for the container.</para>
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="SetVersionsLocationApiCall"/>
        /// <seealso cref="IObjectVersioningExtension.PrepareSetVersionsLocationAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task SetVersionsLocationAsync(this IObjectStorageService service, ContainerName container, ContainerName versionsLocation, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IObjectVersioningExtension extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ObjectVersioning);
            return TaskBlocks.Using(
                () => extension.PrepareSetVersionsLocationAsync(container, versionsLocation, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Disable object versioning for a container.
        /// </summary>
        /// <remarks>
        /// <para>This method is equivalent to calling <see cref="SetVersionsLocationAsync"/> and specifying
        /// <see langword="null"/> for the <c>location</c> parameter.</para>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
#warning consider removing this method
        public static Task RemoveVersionsLocationAsync(this IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return service.SetVersionsLocationAsync(container, null, cancellationToken);
        }

        /// <summary>
        /// Update a generic HTTP API call to include the <see cref="VersionsLocation"/> header, which specifies the
        /// name of the container where old versions of objects in a versioned container get placed when the objects are
        /// updated.
        /// </summary>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="location">
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to clear the value for the <see cref="VersionsLocation"/> header associated
        /// with a container.</para>
        /// </param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="location"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        private static TCall WithVersionsLocationImpl<TCall>(this TCall apiCall, ContainerName location)
            where TCall : IHttpApiRequest
        {
            string encodedLocation = string.Empty;
            if (location != null)
            {
                encodedLocation = location.Value;

                // first, UTF-8 encode the value
                encodedLocation = StorageMetadata.EncodeHeaderValue(encodedLocation);

                // then, URL-encode the value
                encodedLocation = UriUtility.UriEncode(encodedLocation, UriPart.Any);
            }

            apiCall.RequestMessage.Headers.Remove(VersionsLocation);
            apiCall.RequestMessage.Headers.Add(VersionsLocation, encodedLocation);
            return apiCall;
        }

        /// <summary>
        /// Update a generic HTTP API call to include the <see cref="VersionsLocation"/> header, which specifies the
        /// name of the container where old versions of objects in a versioned container get placed when the objects are
        /// updated.
        /// </summary>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare an HTTP API
        /// call.</param>
        /// <param name="location">
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to clear the value for the <see cref="VersionsLocation"/> header associated
        /// with a container.</para>
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="location"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        private static Task<TCall> WithVersionsLocationImpl<TCall>(this Task<TCall> task, ContainerName location)
            where TCall : IHttpApiRequest
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.Select(innerTask => WithVersionsLocationImpl(innerTask.Result, location));
        }
    }
}
