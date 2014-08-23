namespace OpenStack.Services.Identity.V2
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the tenants to which an authentication token has access.
    /// </summary>
    /// <seealso cref="IIdentityService.PrepareListTenantsAsync"/>
    /// <seealso cref="IdentityServiceExtensions.ListTenantsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListTenantsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Tenant>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTenantsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListTenantsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Tenant>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
