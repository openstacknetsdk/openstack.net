namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides the default implementation of the Extract Archive extension to the OpenStack Object Storage
    /// Service V1.
    /// </summary>
    /// <remarks>
    /// <note type="note">
    /// <para>This class should not be instantiated directly. It should be obtained by passing
    /// <see cref="PredefinedObjectStorageExtensions.ExtractArchive"/> to
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/>.</para>
    /// </note>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ExtractArchiveExtension : ServiceExtension<IObjectStorageService>, IExtractArchiveExtension
    {
        /// <summary>
        /// A placeholder container name used in the call to <see cref="IObjectStorageService.PrepareCreateObjectAsync"/>
        /// as part of preparing an Extract Archive API call that does not specify a container name for the extracted
        /// archive content.
        /// </summary>
        private static readonly ContainerName DummyContainerName = new ContainerName("dummyContainer");

        /// <summary>
        /// A placeholder object name used in the call to <see cref="IObjectStorageService.PrepareCreateObjectAsync"/>
        /// as part of preparing an Extract Archive API call that does not specify an object prefix for the extracted
        /// archive content.
        /// </summary>
        private static readonly ObjectName DummyObjectName = new ObjectName("dummyObject");

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveExtension"/> class using the specified Object
        /// Storage Service client and HTTP API call factory.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="httpApiCallFactory">The factory to use for creating new HTTP API calls for the
        /// extension.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="httpApiCallFactory"/> is <see langword="null"/>.</para>
        /// </exception>
        public ExtractArchiveExtension(IObjectStorageService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        /// <inheritdoc/>
        public virtual Task<ExtractArchiveSupportedApiCall> PrepareExtractArchiveSupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareGetObjectStorageInfoAsync(cancellationToken)
                .Select(task => new ExtractArchiveSupportedApiCall(task.Result));
        }

        /// <inheritdoc/>
        public virtual Task<ExtractArchiveApiCall> PrepareExtractArchiveAsync(Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            return PrepareExtractArchiveImplAsync(DummyContainerName, true, DummyObjectName, true, stream, format, cancellationToken, progress);
        }

        /// <inheritdoc/>
        public virtual Task<ExtractArchiveApiCall> PrepareExtractArchiveAsync(ContainerName container, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            return PrepareExtractArchiveImplAsync(container, false, DummyObjectName, true, stream, format, cancellationToken, progress);
        }

        /// <inheritdoc/>
        public virtual Task<ExtractArchiveApiCall> PrepareExtractArchiveAsync(ContainerName container, ObjectName objectPrefix, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            return PrepareExtractArchiveImplAsync(container, false, objectPrefix, false, stream, format, cancellationToken, progress);
        }

        /// <summary>
        /// This method implements general support for the multiple forms an Extract Archive operation can take.
        /// </summary>
        /// <param name="container">The name of the container to place the extracted files in, or a placeholder
        /// container name if <paramref name="removeContainer"/> is <see langword="true"/>.</param>
        /// <param name="removeContainer"><see langword="true"/> if <paramref name="container"/> is a placeholder
        /// container name that should be removed from the final URI before sending the request; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="objectPrefix">The prefix to apply to extracted objects, or a placeholder object prefix if
        /// <paramref name="removeObject"/> is <see langword="true"/>.</param>
        /// <param name="removeObject"><see langword="true"/> if <paramref name="objectPrefix"/> is a placeholder object
        /// name that should be removed from the final URI before sending the request; otherwise,
        /// <see langword="false"/>.</param>
        /// <param name="stream">A stream providing the raw data of the archive to upload and extract.</param>
        /// <param name="format">An <see cref="ArchiveFormat"/> instance describing the file format of the data in
        /// <paramref name="stream"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An <see cref="IProgress{T}"/> instance to notify about progress of sending data to
        /// the Object Storage Service, or <see langword="null"/> if progress updates are not required.</param>
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
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="removeContainer"/> is <see langword="true"/> but <paramref name="removeObject"/> is
        /// <see langword="false"/>.
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        protected virtual Task<ExtractArchiveApiCall> PrepareExtractArchiveImplAsync(ContainerName container, bool removeContainer, ObjectName objectPrefix, bool removeObject, Stream stream, ArchiveFormat format, CancellationToken cancellationToken, IProgress<long> progress)
        {
            return Service.PrepareCreateObjectAsync(container, objectPrefix, stream, cancellationToken, progress)
                .Select(
                    task =>
                    {
                        Uri requestUri = task.Result.RequestMessage.RequestUri;
                        task.Result.RequestMessage.RequestUri = ModifyUri(requestUri, format, removeContainer, removeObject);
                        return task.Result;
                    })
                .Select(task => new ExtractArchiveApiCall(task.Result));
        }

        /// <summary>
        /// Transforms a URI originally created by <see cref="IObjectStorageService.PrepareCreateObjectAsync"/> into the
        /// form required by the Extract Archive operation.
        /// </summary>
        /// <param name="originalUri">
        /// The original URI created by <see cref="IObjectStorageService.PrepareCreateObjectAsync"/>.
        /// </param>
        /// <param name="format">
        /// An <see cref="ArchiveFormat"/> instance describing the format of the archive being uploaded.
        /// </param>
        /// <param name="removeContainer">
        /// <para><see langword="true"/> to remove the container name from the URI.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> to leave the container name path segment in the URI.</para>
        /// <para>For additional information, see the reference documentation for the Extract Archive operation.</para>
        /// </param>
        /// <param name="removeObject">
        /// <para><see langword="true"/> to remove the object name path segment from the URI.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> to leave the object name path segment in the URI.</para>
        /// <para>For additional information, see the reference documentation for the Extract Archive operation.</para>
        /// </param>
        /// <returns>The modified URI.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="originalUri"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="format"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="removeContainer"/> is <see langword="true"/> but <paramref name="removeObject"/> is
        /// <see langword="false"/>.
        /// </exception>
        private static Uri ModifyUri(Uri originalUri, ArchiveFormat format, bool removeContainer, bool removeObject)
        {
            if (originalUri == null)
                throw new ArgumentNullException("originalUri");
            if (format == null)
                throw new ArgumentNullException("format");
            if (removeContainer && !removeObject)
                throw new InvalidOperationException("removeObject must be true if removeContainer is true");

            string originalString = originalUri.OriginalString;
            if (removeObject)
                originalString = ReplaceLastInstance(originalString, "/" + DummyObjectName.Value, string.Empty);
            if (removeContainer)
                originalString = ReplaceLastInstance(originalString, "/" + DummyContainerName.Value, string.Empty);

            Uri uri = new Uri(originalString, originalUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
            return UriUtility.SetQueryParameter(uri, "extract-archive", format.Name);
        }

        /// <summary>
        /// Replaces the last instance of <paramref name="oldValue"/> in the input string
        /// <paramref name="s"/>, if any exists, with the new value <paramref name="newValue"/>.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="oldValue">The string to search for.</param>
        /// <param name="newValue">The string to replace the last instance of <paramref name="oldValue"/> with.</param>
        /// <returns>
        /// <para>A string which is a copy of <paramref name="s"/> with the last instance of <paramref name="oldValue"/>
        /// replaced with <paramref name="newValue"/>.</para>
        /// <para>-or-</para>
        /// <para><paramref name="s"/>, if it does not contain the substring <paramref name="oldValue"/>.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="s"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="oldValue"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="newValue"/> is <see langword="null"/>.</para>
        /// </exception>
        private static string ReplaceLastInstance(string s, string oldValue, string newValue)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (oldValue == null)
                throw new ArgumentNullException("oldValue");
            if (newValue == null)
                throw new ArgumentNullException("newValue");

            int index = s.LastIndexOf(oldValue);
            if (index < 0)
                return s;

            string prefix = s.Substring(0, index);
            string suffix = s.Substring(index + oldValue.Length);
            return prefix + newValue + suffix;
        }
    }
}
