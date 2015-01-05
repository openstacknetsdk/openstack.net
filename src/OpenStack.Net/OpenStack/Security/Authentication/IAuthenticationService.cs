namespace OpenStack.Security.Authentication
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface represents an authentication service capable of locating the base address of services
    /// and authenticating HTTP API requests.
    /// </summary>
    /// <preliminary/>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gets the base address for a specific service.
        /// </summary>
        /// <remarks>
        /// The base address returned by this method may not be an exact match for all arguments to this method.
        /// This method filters the available services in the following order to locate an acceptable match.
        /// If more than one acceptable service remains after all filters are applied, it is unspecified
        /// which one is returned by this method.
        ///
        /// <list type="number">
        /// <item>This method only considers services which match the specified <paramref name="serviceType"/>.</item>
        /// <item>This method attempts to filter the remaining items to those matching <paramref name="serviceName"/>.
        /// If <paramref name="serviceName"/> is <see langword="null"/>, or if no services match the specified name,
        /// <em>this argument is ignored</em>.</item>
        /// <item>This method attempts to filter the remaining items to those matching <paramref name="region"/>. If
        /// <paramref name="region"/> is <see langword="null"/>, the implementation defines the manner in which a
        /// default region (if any) is selected. If no services match the specified region, <em>this argument is
        /// ignored</em>.</item>
        /// <item>If the <paramref name="region"/> argument is ignored as a result of the previous rule, this method
        /// filters the remaining items to only include region-independent services, i.e. services offering a base
        /// address that is not associated with a particular named region.</item>
        /// </list>
        /// </remarks>
        /// <param name="serviceType">The service type to locate.</param>
        /// <param name="serviceName">The preferred name of the service.</param>
        /// <param name="region">The preferred region for the service. If this value is <see langword="null"/>, the
        /// implementation defines the manner in which a default region (if any) is selected.</param>
        /// <param name="internalAddress"><see langword="true"/> to return a base address for accessing the service over
        /// a local network; otherwise, <see langword="false"/> to return a base address for accessing the service over
        /// a public network (the Internet).</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes
        /// successfully, the <see cref="Task{TResult}.Result"/> property will contain a
        ///  <see cref="Uri"/> containing the base address for accessing the service.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        /// <exception cref="NotSupportedException">
        /// If the specified <paramref name="region"/> is not supported.
        /// </exception>
        /// <exception cref="WebException">If an error occurred while sending an HTTP request.</exception>
        Task<Uri> GetBaseAddressAsync(string serviceType, string serviceName, string region, bool internalAddress, CancellationToken cancellationToken);

        /// <summary>
        /// Authenticate an HTTP request.
        /// </summary>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> describing the HTTP request to
        /// authenticate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the specified HTTP request could not be authenticated.
        /// </exception>
        Task AuthenticateRequestAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken);
    }
}
