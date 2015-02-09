namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;

    /// <summary>
    /// This interface defines the primary operations for the Object Versioning extension to the OpenStack Object
    /// Storage Service V1.
    /// </summary>
    /// <remarks>
    /// <para>To obtain an instance of this extension, use the
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/> method to create an instance of the
    /// <see cref="PredefinedObjectStorageExtensions.ObjectVersioning"/> extension.</para>
    /// </remarks>
    /// <preliminary/>
    public interface IObjectVersioningExtension
    {
        /// <summary>
        /// Prepare an API call to create a versioned container in the Object Storage Service, which places a copy of
        /// old versions of objects in a separate container whenever the objects are updated.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="versionsLocation">
        /// The name of the container where old versions of objects in a versioned container get placed when the objects
        /// are updated.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="versionsLocation"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="CreateVersionedContainerApiCall"/>
        /// <seealso cref="ObjectVersioningExtensions.CreateVersionedContainerAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        Task<CreateVersionedContainerApiCall> PrepareCreateVersionedContainerAsync(ContainerName container, ContainerName versionsLocation, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to get the name of the container where old versions of objects in a versioned container
        /// get placed when the objects are updated.
        /// </summary>
        /// <remarks>
        /// <note type="caller">
        /// <para>This method supports the implementation of the object versioning feature for vendors which do not use
        /// the standard <see cref="ObjectVersioningExtensions.VersionsLocation"/> HTTP header for specifying and
        /// enabling object versioning for a container. For OpenStack-compatible vendors, the
        /// <see cref="ObjectVersioningExtensions.GetVersionsLocation"/> method provides more efficient access to the
        /// versions location when the <see cref="ContainerMetadata"/> is already available.</para>
        /// </note>
        /// </remarks>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="GetVersionsLocationApiCall"/>
        /// <seealso cref="ObjectVersioningExtensions.GetVersionsLocation"/>
        /// <seealso cref="ObjectVersioningExtensions.GetVersionsLocationAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        Task<GetVersionsLocationApiCall> PrepareGetVersionsLocationAsync(ContainerName container, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to set the name of the container where old versions of objects in a versioned container
        /// get placed when the objects are updated.
        /// </summary>
        /// <param name="container">The name of the container to update.</param>
        /// <param name="versionsLocation">
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to disable object versioning for the container.</para>
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="SetVersionsLocationApiCall"/>
        /// <seealso cref="ObjectVersioningExtensions.SetVersionsLocationAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/set-object-versions.html">Object versioning (OpenStack Object Storage API V1 Reference)</seealso>
        Task<SetVersionsLocationApiCall> PrepareSetVersionsLocationAsync(ContainerName container, ContainerName versionsLocation, CancellationToken cancellationToken);
    }
}
