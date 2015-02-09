namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class provides extension methods for using the optional Extract Archive functionality
    /// provided by the Object Storage service.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ExtractArchiveExtensions
    {
        /// <summary>
        /// Determines whether a particular Object Storage Service supports the optional Extract Archive operation.
        /// <note type="warning">This method relies on properties which are not defined by OpenStack, and may not be consistent across vendors.</note>
        /// </summary>
        /// <remarks>
        /// If the Object Storage Service supports the Extract Archive operation, but does not support
        /// feature discoverability, this method might return <see langword="false"/> or result in an
        /// <see cref="HttpWebException"/> even though the Extract Archive operation is supported. To
        /// ensure this situation does not prevent the use of the Extract Archive operation, it is not
        /// automatically checked prior to sending an Extract Archive API call.
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes
        /// successfully, the <see cref="Task{TResult}.Result"/> property contains a value
        /// indicating whether or not the service supports the Extract Archive operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/discoverability.html">Discoverability (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<bool> SupportsExtractArchiveAsync(this IObjectStorageService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IExtractArchiveExtension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ExtractArchive);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareExtractArchiveSupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Upload an archive and extract the contained files to create objects in the Object Storage Service.
        /// The root folders in the archive specify the container names in which the objects are created.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="stream">A stream providing the raw data of the archive to upload and extract.</param>
        /// <param name="format">An <see cref="ArchiveFormat"/> instance describing the file format of the data in
        /// <paramref name="stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">
        /// <para>An <see cref="IProgress{T}"/> instance receiving progress updates during the request.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if progress updates are not required.</para>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property contains an <see cref="ExtractArchiveResponse"/> instance
        /// describing the results of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ExtractArchiveResponse> ExtractArchiveAsync(this IObjectStorageService service, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IExtractArchiveExtension extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ExtractArchive);
            return TaskBlocks.Using(
                () => extension.PrepareExtractArchiveAsync(stream, format, cancellationToken, progress),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Upload an archive and extract the contained files to create objects in the Object Storage Service.
        /// The <paramref name="container"/> argument specifies the container into which the extracted objects
        /// are placed.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The name of the container to place the extracted files in.</param>
        /// <param name="stream">A stream providing the raw data of the archive to upload and extract.</param>
        /// <param name="format">An <see cref="ArchiveFormat"/> instance describing the file format of the data in
        /// <paramref name="stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">
        /// <para>An <see cref="IProgress{T}"/> instance receiving progress updates during the request.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if progress updates are not required.</para>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property contains an <see cref="ExtractArchiveResponse"/> instance
        /// describing the results of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ExtractArchiveResponse> ExtractArchiveAsync(this IObjectStorageService service, ContainerName container, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IExtractArchiveExtension extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ExtractArchive);
            return TaskBlocks.Using(
                () => extension.PrepareExtractArchiveAsync(container, stream, format, cancellationToken, progress),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Upload an archive and extract the contained files to create objects in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The name of the container to place the extracted files in.</param>
        /// <param name="objectPrefix">The object name prefix to apply to extracted files as they are extracted into the
        /// container.</param>
        /// <param name="stream">A stream providing the raw data of the archive to upload and extract.</param>
        /// <param name="format">An <see cref="ArchiveFormat"/> instance describing the file format of the data in
        /// <paramref name="stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">
        /// <para>An <see cref="IProgress{T}"/> instance receiving progress updates during the request.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if progress updates are not required.</para>
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property contains an <see cref="ExtractArchiveResponse"/> instance
        /// describing the results of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="objectPrefix"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the Object Storage Service instance does not support the Extract Archive extension.
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ExtractArchiveResponse> ExtractArchiveAsync(this IObjectStorageService service, ContainerName container, ObjectName objectPrefix, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IExtractArchiveExtension extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.ExtractArchive);
            return TaskBlocks.Using(
                () => extension.PrepareExtractArchiveAsync(container, objectPrefix, stream, format, cancellationToken, progress),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }
    }
}
