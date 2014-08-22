namespace OpenStack.Services.Identity
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to obtain information about a specific version of the Identity Service
    /// which is exposed at an endpoint.
    /// </summary>
    /// <seealso cref="IBaseIdentityService.PrepareGetApiVersionAsync"/>
    /// <seealso cref="BaseIdentityServiceExtensions.GetApiVersionAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetApiVersionApiCall : DelegatingHttpApiCall<ApiVersionResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetApiVersionApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetApiVersionApiCall(IHttpApiCall<ApiVersionResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
