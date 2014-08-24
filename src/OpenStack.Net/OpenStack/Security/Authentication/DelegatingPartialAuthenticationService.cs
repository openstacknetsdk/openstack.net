namespace OpenStack.Security.Authentication
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// This class serves as the base class for <see cref="IAuthenticationService"/> implementations which only
    /// authenticate certain HTTP API calls.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class DelegatingPartialAuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// This is the backing field for the <see cref="AuthenticatedCallsService"/> property.
        /// </summary>
        private readonly IAuthenticationService _authenticatedCallsService;

        /// <summary>
        /// This is the backing field for the <see cref="UnauthenticatedCallsService"/> property.
        /// </summary>
        private readonly IAuthenticationService _unauthenticatedCallsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingPartialAuthenticationService"/> class with the
        /// specified delegate authentication service implementations to use for authenticating calls made from a
        /// client.
        /// </summary>
        /// <param name="authenticatedCallsService">
        /// The authentication service to use for authenticated HTTP API calls.
        /// </param>
        /// <param name="unauthenticatedCallsService">
        /// The authentication service to use for unauthenticated HTTP API calls.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="authenticatedCallsService"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="unauthenticatedCallsService"/> is <see langword="null"/>.</para>
        /// </exception>
        protected DelegatingPartialAuthenticationService(IAuthenticationService authenticatedCallsService, IAuthenticationService unauthenticatedCallsService)
        {
            if (authenticatedCallsService == null)
                throw new ArgumentNullException("authenticatedCallsService");
            if (unauthenticatedCallsService == null)
                throw new ArgumentNullException("unauthenticatedCallsService");

            _authenticatedCallsService = authenticatedCallsService;
            _unauthenticatedCallsService = unauthenticatedCallsService;
        }

        /// <summary>
        /// Gets the authentication service to use for authenticated HTTP API calls to this service.
        /// </summary>
        /// <value>The authentication service to use for authenticated HTTP API calls to this service.</value>
        public IAuthenticationService AuthenticatedCallsService
        {
            get
            {
                return _authenticatedCallsService;
            }
        }

        /// <summary>
        /// Gets the authentication service to use for unauthenticated HTTP API calls to this service.
        /// </summary>
        /// <value>The authentication service to use for unauthenticated HTTP API calls to this service.</value>
        public IAuthenticationService UnauthenticatedCallsService
        {
            get
            {
                return _unauthenticatedCallsService;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation calls <see cref="IsAuthenticatedCall"/> to determine whether or not a particular HTTP
        /// API call should be treated as an "authenticated" call. The behavior of this method depends on the result of
        /// that method, as shown in the following table.
        /// <list type="table">
        /// <listheader>
        /// <term><see cref="IsAuthenticatedCall"/></term>
        /// <description>Behavior</description>
        /// </listheader>
        /// <item>
        /// <description><see langword="true"/></description>
        /// <description>Authenticate <paramref name="requestMessage"/> using the <see cref="AuthenticatedCallsService"/> authentication service.</description>
        /// </item>
        /// <item>
        /// <description><see langword="false"/></description>
        /// <description>Authenticate <paramref name="requestMessage"/> using the <see cref="UnauthenticatedCallsService"/> authentication service.</description>
        /// </item>
        /// <item>
        /// <description><see langword="null"/></description>
        /// <description>Return without altering <paramref name="requestMessage"/> at all.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public Task AuthenticateRequestAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage == null)
                throw new ArgumentNullException("requestMessage");

            bool? authenticated = IsAuthenticatedCall(requestMessage);
            if (!authenticated.HasValue)
                return CompletedTask.Default;

            if (authenticated.Value)
                return AuthenticatedCallsService.AuthenticateRequestAsync(requestMessage, cancellationToken);
            else
                return UnauthenticatedCallsService.AuthenticateRequestAsync(requestMessage, cancellationToken);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>The base implementation always uses <see cref="AuthenticatedCallsService"/> to provide the behavior
        /// for this method.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">If a recursive call to this method is detected.</exception>
        public Task<Uri> GetBaseAddressAsync(string serviceType, string serviceName, string region, bool internalAddress, CancellationToken cancellationToken)
        {
            return AuthenticatedCallsService.GetBaseAddressAsync(serviceType, serviceName, region, internalAddress, cancellationToken);
        }

        /// <summary>
        /// Determines whether or not a particular HTTP API call should be treated as an authenticated call or an
        /// unauthenticated call.
        /// </summary>
        /// <param name="requestMessage">The <see cref="HttpRequestMessage"/> representing the HTTP API call.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="requestMessage"/> represents an authenticated HTTP API
        /// call.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if <paramref name="requestMessage"/> represents an unauthenticated HTTP API
        /// call.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="requestMessage"/> is not a recognized HTTP API call (see
        /// <see cref="AuthenticateRequestAsync"/> for additional information).</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="requestMessage"/> is <see langword="null"/>.
        /// </exception>
        protected abstract bool? IsAuthenticatedCall(HttpRequestMessage requestMessage);
    }
}
