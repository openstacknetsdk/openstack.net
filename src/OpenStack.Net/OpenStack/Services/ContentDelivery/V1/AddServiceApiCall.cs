namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to add a <see cref="Service"/> resource in the Content Delivery Service.
    /// </summary>
    /// <seealso cref="IContentDeliveryService.PrepareAddServiceAsync"/>
    /// <seealso cref="ContentDeliveryServiceExtensions.AddServiceAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddServiceApiCall : DelegatingHttpApiCall<ServiceId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddServiceApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public AddServiceApiCall(IHttpApiCall<ServiceId> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
