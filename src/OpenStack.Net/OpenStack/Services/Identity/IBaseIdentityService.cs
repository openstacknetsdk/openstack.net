namespace OpenStack.Services.Identity
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// This is the base interface for the OpenStack Identity Service. It provides the ability to obtain details about
    /// the version(s) of the Identity Service which are exposed at the current endpoint.
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html">Identity API v2.0 (OpenStack Complete API Reference)</seealso>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html">Identity API v3 (OpenStack Complete API Reference)</seealso>
    /// <preliminary/>
    public interface IBaseIdentityService : IHttpService
    {
        /// <summary>
        /// Prepare an HTTP API call to obtain a list of API versions available at the current endpoint for a service.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a <see cref="ListApiVersionsApiCall"/> instance
        /// describing the HTTP API call.
        /// </returns>
        /// <seealso cref="BaseIdentityServiceExtensions.ListApiVersionsAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-v2-versions">API versions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html#versions-identity-v3">API versions (Identity API v3 - OpenStack Complete API Reference)</seealso>
        Task<ListApiVersionsApiCall> PrepareListApiVersionsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an HTTP API call to obtain information about a particular version of the API available at the
        /// current endpoint for the service.
        /// </summary>
        /// <param name="apiVersionId"></param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a <see cref="GetApiVersionApiCall"/> instance
        /// describing the HTTP API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="apiVersionId"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="BaseIdentityServiceExtensions.GetApiVersionAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-v2-versions">API versions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html#versions-identity-v3">API versions (Identity API v3 - OpenStack Complete API Reference)</seealso>
        Task<GetApiVersionApiCall> PrepareGetApiVersionAsync(ApiVersionId apiVersionId, CancellationToken cancellationToken);
    }
}
