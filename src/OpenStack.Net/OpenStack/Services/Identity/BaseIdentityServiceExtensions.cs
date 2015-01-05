namespace OpenStack.Services.Identity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class defines extension methods for simplifying the use of the <seealso cref="IBaseIdentityService"/>
    /// service in the "common" usage scenarios.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class BaseIdentityServiceExtensions
    {
        /// <summary>
        /// Prepare and send an HTTP API call to obtain a list of API versions available at the current endpoint for a
        /// service.
        /// </summary>
        /// <param name="service">The <see cref="IBaseIdentityService"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain the first page of results describing the API
        /// versions available at the current endpoint.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="service"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurred during an HTTP request while preparing or sending the HTTP API call.
        /// </exception>
        /// <seealso cref="IBaseIdentityService.PrepareListApiVersionsAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-v2-versions">API versions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html#versions-identity-v3">API versions (Identity API v3 - OpenStack Complete API Reference)</seealso>
        public static Task<ReadOnlyCollectionPage<ApiVersion>> ListApiVersionsAsync(this IBaseIdentityService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListApiVersionsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an HTTP API call to obtain information about a particular version of the API available at
        /// the current endpoint for the service.
        /// </summary>
        /// <param name="service">The <see cref="IBaseIdentityService"/> instance.</param>
        /// <param name="apiVersionId">The unique ID of the API version.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain an <see cref="ApiVersion"/> instance describing the
        /// specified version of the API.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="apiVersionId"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurred during an HTTP request while preparing or sending the HTTP API call.
        /// </exception>
        /// <seealso cref="BaseIdentityServiceExtensions.GetApiVersionAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-v2-versions">API versions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html#versions-identity-v3">API versions (Identity API v3 - OpenStack Complete API Reference)</seealso>
        public static Task<ApiVersion> GetApiVersionAsync(this IBaseIdentityService service, ApiVersionId apiVersionId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (apiVersionId == null)
                throw new ArgumentNullException("apiVersionId");

            return TaskBlocks.Using(
                () => service.PrepareGetApiVersionAsync(apiVersionId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Version));
        }
    }
}
