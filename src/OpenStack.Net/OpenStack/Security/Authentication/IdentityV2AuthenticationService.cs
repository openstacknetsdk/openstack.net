namespace OpenStack.Security.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using OpenStack.Services.Identity;
    using OpenStack.Services.Identity.V2;
    using Rackspace.Threading;

    /// <summary>
    /// This class defines an authentication service based on the OpenStack Identity Service V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class IdentityV2AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// This is the backing field for the <see cref="IdentityService"/> property.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// This is the backing field for the <see cref="AuthenticationRequest"/> property.
        /// </summary>
        private readonly AuthenticationRequest _authenticationRequest;

        /// <summary>
        /// This is the backing field for the <see cref="Access"/> property.
        /// </summary>
        private Access _access;

        /// <summary>
        /// This is the backing field for the <see cref="ExpirationOverlap"/> property.
        /// </summary>
        private TimeSpan _expirationOverlap = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityV2AuthenticationService"/> class with the specified
        /// OpenStack Identity Service V2 client and prepared authentication request.
        /// </summary>
        /// <param name="identityService">The <see cref="IIdentityService"/> instance to use for authentication
        /// purposes.</param>
        /// <param name="authenticationRequest">The authentication request, which contains the credentials to use for
        /// authenticating with the OpenStack Identity Service V2.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="identityService"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="authenticationRequest"/> is <see langword="null"/>.</para>
        /// </exception>
        public IdentityV2AuthenticationService(IIdentityService identityService, AuthenticationRequest authenticationRequest)
        {
            if (identityService == null)
                throw new ArgumentNullException("identityService");
            if (authenticationRequest == null)
                throw new ArgumentNullException("authenticationRequest");

            _identityService = identityService;
            _authenticationRequest = authenticationRequest;
        }

        /// <summary>
        /// Gets or sets the overlap to consider for re-authenticating tokens that are about to expire. The default
        /// value is 5 minutes.
        /// </summary>
        /// <returns>
        /// <para>The expiration overlap for tokens provided by the OpenStack Identity Service V2. If the time until a token
        /// expires is less than this value, it will be treated as already expired and the Identity API will be used to
        /// re-authenticate the user prior to authenticating additional HTTP API calls.</para>
        /// </returns>
        /// <value>
        /// <para>The overlap to consider for re-authenticating tokens that are about to expire.</para>
        /// </value>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is negative.</exception>
        public TimeSpan ExpirationOverlap
        {
            get
            {
                return _expirationOverlap;
            }

            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value");

                _expirationOverlap = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IIdentityService"/> instance to use for authentication requests.
        /// </summary>
        /// <value>The <see cref="IIdentityService"/> instance to use for authentication requests.</value>
        protected IIdentityService IdentityService
        {
            get
            {
                return _identityService;
            }
        }

        /// <summary>
        /// Gets the prepared <see cref="OpenStack.Services.Identity.V2.AuthenticationRequest"/> to use for
        /// authentication purposes.
        /// </summary>
        /// <value>The prepared <see cref="OpenStack.Services.Identity.V2.AuthenticationRequest"/> to use for
        /// authentication purposes.</value>
        protected AuthenticationRequest AuthenticationRequest
        {
            get
            {
                return _authenticationRequest;
            }
        }

        /// <summary>
        /// Gets the cached result of authenticating a user, improving the efficiency of locating multiple services in
        /// the service catalog.
        /// </summary>
        /// <remarks>
        /// <para>This property may be used to improve the efficiency of locating multiple services within the service
        /// catalog, as well as avoiding multiple HTTP API calls to retrieve the <see cref="TokenId"/> necessary for
        /// authenticating other API calls.</para>
        /// <note type="note">
        /// <para>The value of this property is not checked against its own <see cref="Token.ExpiresAt"/> property or
        /// against the <see cref="ExpirationOverlap"/> used by this client.</para>
        /// </note>
        /// </remarks>
        /// <value>
        /// <para>The cached result of authenticating a user.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if no cached <see cref="OpenStack.Services.Identity.V2.Access"/> value is
        /// currently available.</para>
        /// </value>
        protected Access Access
        {
            get
            {
                return _access;
            }
        }

        #region IAuthenticationService Members

        /// <inheritdoc/>
        /// <remarks>
        /// <para>The base implementation calls <see cref="AuthenticateAsync"/> to authenticate with the OpenStack
        /// Identity Service V2 if necessary, followed by calling <see cref="GetBaseAddressImpl"/> to locate a service
        /// endpoint within the user's service catalog.</para>
        /// </remarks>
        public virtual Task<Uri> GetBaseAddressAsync(string serviceType, string serviceName, string region, bool internalAddress, CancellationToken cancellationToken)
        {
            return
                AuthenticateAsync(cancellationToken)
                .Select(task => GetBaseAddressImpl(task.Result, serviceType, serviceName, region, internalAddress));
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>The base implementation of this authentication client sets the <c>X-Auth-Token</c> HTTP header of
        /// requests to the <see cref="TokenId"/> value obtained by authenticating with the OpenStack Identity Service
        /// V2.</para>
        /// </remarks>
        public virtual Task AuthenticateRequestAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            return
                AuthenticateAsync(cancellationToken)
                .Select(
                    task =>
                    {
                        Access access = task.Result;
                        if (access == null)
                            return;

                        Token token = access.Token;
                        if (token == null)
                            return;

                        if (token.Id == null)
                            return;

                        requestMessage.Headers.Add("X-Auth-Token", token.Id.Value);
                    });
        }

        #endregion

        /// <summary>
        /// Authenticates the credentials provided in <see cref="AuthenticationRequest"/> with the OpenStack Identity
        /// Service V2.
        /// </summary>
        /// <remarks>
        /// <para>This method caches the authentication result, and returns the cached result of a previous
        /// authentication request when possible to avoid unnecessary calls to the Identity API. If a cached
        /// authentication result is available but has either expired or will expire soon (see
        /// <see cref="ExpirationOverlap"/>), the cached result is discarded and the credentials are re-authenticated
        /// with the Identity Service.</para>
        /// </remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully, the
        /// <see cref="Task{TResult}.Result"/> property will contain an <see cref="Access"/> instance providing the
        /// authentication result.
        /// </returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of authenticating with the Identity API.
        /// </exception>
        protected virtual Task<Access> AuthenticateAsync(CancellationToken cancellationToken)
        {
            Access access = Access;
            if (access != null && access.Token != null)
            {
                Token token = access.Token;
                // Note: this code uses lifting to null to cover the case where token.ExpiresAt is null
                DateTimeOffset? effectiveExpiration = token.ExpiresAt - ExpirationOverlap;
                if (effectiveExpiration > DateTimeOffset.Now)
                    return CompletedTask.FromResult(access);
            }

            return
                IdentityService.AuthenticateAsync(AuthenticationRequest, cancellationToken)
                .Select(
                    task =>
                    {
                        _access = task.Result;
                        return task.Result;
                    });
        }

        /// <summary>
        /// This method provides the core implementation of <see cref="GetBaseAddressAsync"/> after the
        /// <see cref="OpenStack.Services.Identity.V2.Access"/> details are obtained from the Identity Service.
        /// </summary>
        /// <param name="access">An <see cref="OpenStack.Services.Identity.V2.Access"/> object containing the details
        /// for the authenticated user.</param>
        /// <param name="serviceType">The service type to locate.</param>
        /// <param name="serviceName">The preferred name of the service.</param>
        /// <param name="region">The preferred region for the service. This method calls
        /// <see cref="GetEffectiveRegion"/> with this value to obtain the actual region to consider for this
        /// algorithm.</param>
        /// <param name="internalAddress">
        /// <para><see langword="true"/> to return a base address for accessing the service over a local network.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> to return a base address for accessing the service over a public network (the
        /// Internet).</para></param>
        /// <returns>A <see cref="Uri"/> containing the absolute base address for accessing the service.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="access"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="serviceType"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="serviceType"/> is empty.</exception>
        protected virtual Uri GetBaseAddressImpl(Access access, string serviceType, string serviceName, string region, bool internalAddress)
        {
            if (access == null)
                throw new ArgumentNullException("access");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (string.IsNullOrEmpty(serviceType))
                throw new ArgumentException("serviceType cannot be empty", "serviceType");

            if (access.ServiceCatalog == null)
                throw new InvalidOperationException("The authentication information provided by the Identity Service did not include a service catalog.");

            List<ServiceCatalogEntry> services = access.ServiceCatalog.Where(sc => string.Equals(sc.Type, serviceType, StringComparison.OrdinalIgnoreCase)).ToList();
            if (services.Count == 0)
                throw new InvalidOperationException(string.Format("The service catalog provided by the Identity Service did not include any service with the type '{0}'.", serviceType));

            if (serviceName != null)
            {
                // If any service matches the preferred name, filter the entire list. Otherwise, simply ignore the
                // preferred name and continue.
                List<ServiceCatalogEntry> namedServices = services.Where(sc => string.Equals(sc.Name, serviceName, StringComparison.OrdinalIgnoreCase)).ToList();
                if (namedServices.Count > 0)
                    services = namedServices;
            }

            // Treat each endpoint individually for the purpose of endpoint selection.
            List<Tuple<ServiceCatalogEntry, Endpoint>> endpoints =
                services.SelectMany(service => service.Endpoints.Select(endpoint => Tuple.Create(service, endpoint))).ToList();

            string effectiveRegion = GetEffectiveRegion(access, region);

            // Locate all endpoints in the effective region. (Note that null and empty are equivalent for regions.)
            List<Tuple<ServiceCatalogEntry, Endpoint>> regionEndpoints =
                endpoints.Where(i => string.Equals(i.Item2.Region ?? string.Empty, effectiveRegion ?? string.Empty, StringComparison.OrdinalIgnoreCase)).ToList();

            // Use filtered results if possible, otherwise only consider "global" endpoints.
            if (regionEndpoints.Count > 0)
                endpoints = regionEndpoints;
            else
                endpoints.RemoveAll(i => !string.IsNullOrEmpty(i.Item2.Region));

            if (effectiveRegion == null && !endpoints.Any())
                throw new InvalidOperationException("No region was provided, no default region is available for the current credentials, and the service does not provide a region-independent endpoint.");

            Tuple<ServiceCatalogEntry, Endpoint> serviceEndpoint = endpoints.FirstOrDefault();
            if (internalAddress)
                serviceEndpoint = endpoints.FirstOrDefault(i => i.Item2.InternalUrl != null);
            else
                serviceEndpoint = endpoints.FirstOrDefault(i => i.Item2.PublicUrl != null);

            if (serviceEndpoint == null)
                throw new InvalidOperationException("No endpoint matching the specified parameters was located in the service catalog.");

            Uri baseAddress;
            if (internalAddress)
                baseAddress = serviceEndpoint.Item2.InternalUrl;
            else
                baseAddress = serviceEndpoint.Item2.PublicUrl;

            Uri adjustedBaseAddress = FilterBaseAddress(baseAddress);
            return adjustedBaseAddress;
        }

        /// <summary>
        /// This method allows an authentication service to adjust the raw URI provided as a base address in the service
        /// catalog prior to its use in resolving HTTP API calls.
        /// </summary>
        /// <remarks>
        /// <para>The default implementation ensures that if the base address is an absolute URI, then it includes a
        /// trailing <c>/</c> character (adding one if necessary). This adjusts for a common case where a trailing
        /// <c>/</c> is omitted from a non-empty <see cref="Uri.AbsolutePath"/> of a URI in the service catalog. In this
        /// case, the use of RFC 3986 §5.2 for resolving relative URIs would result in the unintentional omission of the
        /// final path segment in the base address from the final absolute URI for an HTTP API call.</para>
        /// </remarks>
        /// <param name="baseAddress">The base address for a service as provided in the service catalog.</param>
        /// <returns>The effective base address to use for HTTP API calls to the service. This may be
        /// <paramref name="baseAddress"/> itself, or any altered form as appropriate for the authentication service
        /// and/or vendor currently being accessed.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        protected virtual Uri FilterBaseAddress(Uri baseAddress)
        {
            if (baseAddress == null)
                throw new ArgumentNullException("baseAddress");

            if (!baseAddress.IsAbsoluteUri)
                return baseAddress;

            if (!string.IsNullOrEmpty(baseAddress.Fragment))
                return baseAddress;

            if (baseAddress.AbsoluteUri.EndsWith("/"))
                return baseAddress;

            Uri adjusted = new Uri(baseAddress.AbsoluteUri + "/", UriKind.Absolute);
            return adjusted;
        }

        /// <summary>
        /// Gets the effective region to use for locating services in the service catalog, for the specified
        /// <see cref="OpenStack.Services.Identity.V2.Access"/> information and preferred <paramref name="region"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation simply returns <paramref name="region"/>. Specific vendors may extend this
        /// functionality to provide a default value or other region selections as appropriate for their users and
        /// service offerings.
        /// </remarks>
        /// <param name="access">The <see cref="OpenStack.Services.Identity.V2.Access"/> object providing details for
        /// the authenticated user.</param>
        /// <param name="region">The preferred region, as specified in the call to
        /// <see cref="GetBaseAddressAsync"/>.</param>
        /// <returns>The effective region to use for service location in <see cref="GetBaseAddressImpl"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="access"/> is <see langword="null"/>.</exception>
        protected virtual string GetEffectiveRegion(Access access, string region)
        {
            if (access == null)
                throw new ArgumentNullException("access");

            return region;
        }
    }
}
