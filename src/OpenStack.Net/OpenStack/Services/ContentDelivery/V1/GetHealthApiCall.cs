namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder/> in the Content Delivery Service.
    /// </summary>
    /// <seealso cref="IContentDeliveryService.PrepareGetHealthAsync"/>
    /// <seealso cref="ContentDeliveryServiceExtensions.GetHealthAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetHealthApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHealthApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetHealthApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
