namespace OpenStack.Services.Identity.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using OpenStack.Net;
    using Rackspace.Threading;


    /// <summary>
    /// This class defines extension methods for simplifying the use of the <seealso cref="IIdentityService"/> service
    /// in the "common" usage scenarios.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class IdentityServiceExtensions
    {
        #region Extensions

        /// <summary>
        /// Prepare and send an HTTP API call to list the extensions available for the current OpenStack Identity
        /// Service V2 endpoint.
        /// </summary>
        /// <param name="service">The <see cref="IIdentityService"/> instance.</param>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain the first page of a collection of
        /// <see cref="Extension"/> objects describing the extensions available for the current OpenStack Identity
        /// Service V2 endpoint.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing or sending the API call.
        /// </exception>
        /// <seealso cref="IIdentityService.PrepareListExtensionsAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-api-extensions">Extensions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        public static Task<ReadOnlyCollectionPage<Extension>> ListExtensionsAsync(this IIdentityService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListExtensionsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an HTTP API call to obtain details for a specific extension available for the current
        /// OpenStack Identity Service V2 endpoint.
        /// </summary>
        /// <param name="service">The <see cref="IIdentityService"/> instance.</param>
        /// <param name="alias">The unique alias identifying the extension.</param>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain an <see cref="Extension"/> instance describing the
        /// extension.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="alias"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing or sending the API call.
        /// </exception>
        /// <seealso cref="IIdentityService.PrepareGetExtensionAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-api-extensions">Extensions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        public static Task<Extension> GetExtensionAsync(this IIdentityService service, ExtensionAlias alias, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (alias == null)
                throw new ArgumentNullException("alias");

            return TaskBlocks.Using(
                () => service.PrepareGetExtensionAsync(alias, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Extension));
        }

        #endregion

        #region Tokens

        /// <summary>
        /// Prepare and send an HTTP API call to obtain a authenticate a set of credentials with the OpenStack Identity
        /// Service V2.
        /// </summary>
        /// <param name="service">The <see cref="IIdentityService"/> instance.</param>
        /// <param name="request">An <seealso cref="AuthenticationRequest"/> object containing the credentials to
        /// authenticate.</param>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain an <see cref="Access"/> instance containing the
        /// authentication details.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing or sending the API call.
        /// </exception>
        /// <seealso cref="IIdentityService.PrepareAuthenticateAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-auth-v2">Tokens (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        public static Task<Access> AuthenticateAsync(this IIdentityService service, AuthenticationRequest request, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareAuthenticateAsync(request, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Access));
        }

        /// <summary>
        /// Prepare and send an HTTP API call to list the tenants to which an authentication token has access.
        /// </summary>
        /// <param name="service">The <see cref="IIdentityService"/> instance.</param>
        /// <param name="cancellationToken">The <seealso cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <seealso cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain the first page of a collection of
        /// <see cref="Tenant"/> objects describing the tenants to which an authentication token has access.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing or sending the API call.
        /// </exception>
        /// <seealso cref="IIdentityService.PrepareListTenantsAsync"/>
        /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-auth-v2">Tokens (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
        public static Task<ReadOnlyCollectionPage<Tenant>> ListTenantsAsync(this IIdentityService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListTenantsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        #endregion
    }
}
