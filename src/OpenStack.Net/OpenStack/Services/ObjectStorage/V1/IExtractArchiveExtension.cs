namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Stream = System.IO.Stream;

#if !NET40PLUS
    using Rackspace.Threading;
#endif

    /// <summary>
    /// This interface defines the primary operations for the Extract Archive extension to the OpenStack Object Storage
    /// Service V1.
    /// </summary>
    /// <remarks>
    /// <para>To obtain an instance of this extension, use the
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/> method to create an instance of the
    /// <see cref="PredefinedObjectStorageExtensions.ExtractArchive"/> extension.</para>
    /// </remarks>
    /// <preliminary/>
    public interface IExtractArchiveExtension
    {
        /// <summary>
        /// Prepare an API call to determine whether a particular Object Storage Service supports the optional Extract
        /// Archive operation.
        /// <note type="warning">This method relies on properties which are not defined by OpenStack, and may not be
        /// consistent across vendors.</note>
        /// </summary>
        /// <remarks>
        /// <para>If the Object Storage Service supports the Extract Archive operation, but does not support feature
        /// discoverability, this method might return <see langword="false"/> or result in an
        /// <see cref="HttpWebException"/> even though the Extract Archive operation is supported. To ensure this
        /// situation does not prevent the use of the Extract Archive operation, it is not automatically checked prior
        /// to sending an Extract Archive API call.</para>
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/discoverability.html">Discoverability (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        Task<ExtractArchiveSupportedApiCall> PrepareExtractArchiveSupportedAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to upload an archive and extract the contained files to create objects in the Object
        /// Storage Service. The root folders in the archive specify the container names in which the objects are
        /// created.
        /// </summary>
        /// <param name="stream">A stream providing the raw data of the archive to upload and extract.</param>
        /// <param name="format">An <see cref="ArchiveFormat"/> instance describing the file format of the data in
        /// <paramref name="stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">
        /// <para>An <see cref="IProgress{T}"/> instance receiving progress updates during the request.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if progress updates are not required.</para>
        /// </param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        Task<ExtractArchiveApiCall> PrepareExtractArchiveAsync(Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress);

        /// <summary>
        /// Prepare an API call to upload an archive and extract the contained files to create objects in the Object
        /// Storage Service. The <paramref name="container"/> argument specifies the container into which the extracted
        /// objects are placed.
        /// </summary>
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
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        Task<ExtractArchiveApiCall> PrepareExtractArchiveAsync(ContainerName container, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress);

        /// <summary>
        /// Prepare an API call to upload an archive and extract the contained files to create objects in the Object
        /// Storage Service.
        /// </summary>
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
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="objectPrefix"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
        Task<ExtractArchiveApiCall> PrepareExtractArchiveAsync(ContainerName container, ObjectName objectPrefix, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress);
    }
}
