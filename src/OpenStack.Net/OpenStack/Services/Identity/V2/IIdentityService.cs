namespace OpenStack.Services.Identity.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;

    /// <summary>
    /// This interface extends the base Identity Service with HTTP API calls for performing client authentication with
    /// the Identity Service V2.
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html">Identity API v2.0 (OpenStack Complete API Reference)</seealso>
    /// <preliminary/>
    public interface IIdentityService : IBaseIdentityService
    {
        #region Extensions

        /// <summary>
        /// Prepare an HTTP API call to list the extensions available for the current OpenStack Identity Service V2
        /// endpoint.
        /// </summary>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a <see cref="ListExtensionsApiCall"/> instance
        /// describing the HTTP API call.
        /// </returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="IdentityServiceExtensions.ListExtensionsAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-api-extensions">Extensions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        Task<ListExtensionsApiCall> PrepareListExtensionsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an HTTP API call to obtain details for a specific extension available for the current OpenStack
        /// Identity Service V2 endpoint.
        /// </summary>
        /// <param name="alias">The unique alias identifying the extension.</param>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a <see cref="GetExtensionApiCall"/> instance
        /// describing the HTTP API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="alias"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="IdentityServiceExtensions.GetExtensionAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-api-extensions">Extensions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        Task<GetExtensionApiCall> PrepareGetExtensionAsync(ExtensionAlias alias, CancellationToken cancellationToken);

        #endregion

        #region Tokens

        /// <summary>
        /// Prepare an HTTP API call to obtain a authenticate a set of credentials with the OpenStack Identity Service
        /// V2.
        /// </summary>
        /// <param name="request">An <seealso cref="AuthenticationRequest"/> object containing the credentials to
        /// authenticate.</param>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a <see cref="AuthenticateApiCall"/> instance
        /// describing the HTTP API call.
        /// </returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="IdentityServiceExtensions.AuthenticateAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-auth-v2">Tokens (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        Task<AuthenticateApiCall> PrepareAuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an HTTP API call to list the tenants to which an authentication token has access.
        /// </summary>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain a <see cref="ListTenantsApiCall"/> instance
        /// describing the HTTP API call.
        /// </returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="IdentityServiceExtensions.ListTenantsAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-auth-v2">Tokens (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        Task<ListTenantsApiCall> PrepareListTenantsAsync(CancellationToken cancellationToken);

        #endregion
    }
}
