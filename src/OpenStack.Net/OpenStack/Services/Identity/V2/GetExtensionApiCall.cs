namespace OpenStack.Services.Identity.V2
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to obtain details for a specific extension available for the current
    /// OpenStack Identity Service V2 endpoint.
    /// </summary>
    /// <seealso cref="IIdentityService.PrepareGetExtensionAsync"/>
    /// <seealso cref="IdentityServiceExtensions.GetExtensionAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetExtensionApiCall : DelegatingHttpApiCall<ExtensionResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetExtensionApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetExtensionApiCall(IHttpApiCall<ExtensionResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
