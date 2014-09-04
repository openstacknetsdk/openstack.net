namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides extension methods for using the optional Temporary URL functionality provided by the Object
    /// Storage service.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html">Temporary URL middleware (OpenStack Object Storage API v1 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class TemporaryUriExtensions
    {
        /// <summary>
        /// Determines whether a particular Object Storage Service supports the optional Temporary URL middleware
        /// extension.
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
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property contains a value indicating whether or not the service supports
        /// the Temporary URL middleware extension.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/discoverability.html">Discoverability (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html">Temporary URL middleware (OpenStack Object Storage API v1 Reference)</seealso>
        public static Task<bool> SupportsTemporaryUriAsync(this IObjectStorageService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ITemporaryUriExtension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedObjectStorageExtensions.TemporaryUri);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareTemporaryUriSupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Construct a <see cref="Uri"/> supporting public access to a specific object in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
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
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
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
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="AccountMetadataExtensions.AccountSecretKey"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html">Temporary URL middleware (OpenStack Object Storage API v1 Reference)</seealso>
        public static Task<Uri> CreateTemporaryUriAsync(this IObjectStorageService service, HttpMethod method, ContainerName container, ObjectName @object, string key, DateTimeOffset expiration, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ITemporaryUriExtension temporaryUriExtension = service.GetServiceExtension(PredefinedObjectStorageExtensions.TemporaryUri);
            return temporaryUriExtension.CreateTemporaryUriAsync(method, container, @object, key, expiration, cancellationToken);
        }
    }
}
