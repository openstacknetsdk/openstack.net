namespace OpenStack.Services.Identity
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to obtain information about the versions of the Identity Service
    /// which are exposed at an endpoint.
    /// </summary>
    /// <seealso cref="IBaseIdentityService.PrepareListApiVersionsAsync"/>
    /// <seealso cref="BaseIdentityServiceExtensions.ListApiVersionsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListApiVersionsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<ApiVersion>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListApiVersionsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListApiVersionsApiCall(IHttpApiCall<ReadOnlyCollectionPage<ApiVersion>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
