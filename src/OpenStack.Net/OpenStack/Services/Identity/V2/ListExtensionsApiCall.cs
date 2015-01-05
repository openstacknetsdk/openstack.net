namespace OpenStack.Services.Identity.V2
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the extensions available for the current OpenStack Identity
    /// Service V2 endpoint.
    /// </summary>
    /// <seealso cref="IIdentityService.PrepareListExtensionsAsync"/>
    /// <seealso cref="IdentityServiceExtensions.ListExtensionsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListExtensionsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Extension>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListExtensionsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListExtensionsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Extension>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
