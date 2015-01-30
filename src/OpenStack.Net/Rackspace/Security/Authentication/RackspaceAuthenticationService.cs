namespace Rackspace.Security.Authentication
{
    using System;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity.V2;
    using Rackspace.Services.Identity.V2;

    /// <summary>
    /// This class extends the OpenStack <see cref="IdentityV2AuthenticationService"/> with support for the
    /// Rackspace-specific service catalog and default region supported by Cloud Identity accounts.
    /// </summary>
    /// <remarks>
    /// <para>The default region for an authenticated user is only considered when a region is not explicitly specified
    /// in the construction of a service client.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RackspaceAuthenticationService : IdentityV2AuthenticationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RackspaceAuthenticationService"/> class with the specified
        /// identity service and prepared authentication request.
        /// </summary>
        /// <param name="identityService">
        /// The <see cref="IIdentityService"/> instance to use for authentication purposes.
        /// </param>
        /// <param name="authenticationRequest">The authentication request, which contains the credentials to use for
        /// authenticating with Rackspace Cloud Identity.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="identityService"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="authenticationRequest"/> is <see langword="null"/>.</para>
        /// </exception>
        public RackspaceAuthenticationService(IIdentityService identityService, AuthenticationRequest authenticationRequest)
            : base(identityService, authenticationRequest)
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method updates the <paramref name="serviceType"/> and/or <paramref name="serviceName"/> values for
        /// cases where "vanilla" OpenStack values were specified but Rackspace exposes the compatible services under
        /// different names in the service catalog.
        /// </remarks>
        protected override Uri GetBaseAddressImpl(Access access, string serviceType, string serviceName, string region, bool internalAddress)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                switch (serviceType)
                {
                case "compute":
                    serviceName = "cloudServersOpenStack";
                    break;

                case "volume":
                    serviceName = "cloudBlockStorage";
                    break;

                case "object-store":
                    serviceName = "cloudFiles";
                    break;

                case "database":
                    serviceType = "rax:database";
                    serviceName = "cloudDatabases";
                    break;

                case "queuing":
                    serviceType = "rax:queues";
                    serviceName = "cloudQueues";
                    break;
                }
            }

            return base.GetBaseAddressImpl(access, serviceType, serviceName, region, internalAddress);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation returns <paramref name="region"/> if it is non-<see langword="null"/>. If
        /// <paramref name="region"/> is <see langword="null"/>, the default region for the specified
        /// <see cref="Access"/> is returned if available; otherwise, this method returns <see langword="null"/>.
        /// </remarks>
        protected override string GetEffectiveRegion(Access access, string region)
        {
            string effectiveRegion = base.GetEffectiveRegion(access, region);
            if (effectiveRegion == null)
                effectiveRegion = access.User.GetDefaultRegion();

            return effectiveRegion;
        }
    }
}
