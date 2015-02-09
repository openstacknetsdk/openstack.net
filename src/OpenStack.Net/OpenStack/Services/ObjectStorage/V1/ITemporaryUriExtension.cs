namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;

    /// <summary>
    /// This interface defines the primary operations for the Temporary URL middleware extension to the OpenStack Object
    /// Storage Service V1.
    /// </summary>
    /// <remarks>
    /// <para>To obtain an instance of this extension, use the
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/> method to create an instance of the
    /// <see cref="PredefinedObjectStorageExtensions.TemporaryUri"/> extension.</para>
    /// </remarks>
    /// <preliminary/>
    public interface ITemporaryUriExtension
    {
        /// <summary>
        /// Prepare an API call to determine whether a particular Object Storage Service supports the optional Temporary
        /// URL middleware.
        /// <note type="warning">This method relies on properties which are not defined by OpenStack, and may not be
        /// consistent across vendors.</note>
        /// </summary>
        /// <remarks>
        /// <para>If the Object Storage Service supports the Temporary URL middleware, but does not support feature
        /// discoverability, this method might return <see langword="false"/> or result in an
        /// <see cref="HttpWebException"/> even though the Temporary URL feature is supported. To ensure this situation
        /// does not prevent the use of the Temporary URL functionality, it is not automatically checked during the
        /// <see cref="CreateTemporaryUriAsync"/> operation.</para>
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/discoverability.html">Discoverability (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html">Temporary URL middleware (OpenStack Object Storage API v1 Reference)</seealso>
        Task<TemporaryUriSupportedApiCall> PrepareTemporaryUriSupportedAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Construct a <see cref="Uri"/> supporting public access to a specific object in the Object Storage Service.
        /// </summary>
        /// <param name="method">The HTTP method which can be used to access the object.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="key">The account key to use with the Temporary URL middleware extension, as specified in the
        /// account <see cref="AccountMetadataExtensions.AccountSecretKey"/> metadata.</param>
        /// <param name="expiration">The expiration time for the generated URI.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a the absolute URI which may be used to access the
        /// specified object, which is valid until the <paramref name="expiration"/> time passes or the account key is
        /// changed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="method"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="key"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>If <paramref name="key"/> is empty.</para>
        /// </exception>
        /// <seealso cref="AccountMetadataExtensions.AccountSecretKey"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html">Temporary URL middleware (OpenStack Object Storage API v1 Reference)</seealso>
        Task<Uri> CreateTemporaryUriAsync(HttpMethod method, ContainerName container, ObjectName @object, string key, DateTimeOffset expiration, CancellationToken cancellationToken);
    }
}
