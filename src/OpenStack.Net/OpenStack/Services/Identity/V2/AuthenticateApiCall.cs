namespace OpenStack.Services.Identity.V2
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to authenticate a set of credentials with the OpenStack Identity Service
    /// V2.
    /// </summary>
    /// <seealso cref="IIdentityService.PrepareAuthenticateAsync"/>
    /// <seealso cref="IdentityServiceExtensions.AuthenticateAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AuthenticateApiCall : DelegatingHttpApiCall<AccessResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public AuthenticateApiCall(IHttpApiCall<AccessResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
