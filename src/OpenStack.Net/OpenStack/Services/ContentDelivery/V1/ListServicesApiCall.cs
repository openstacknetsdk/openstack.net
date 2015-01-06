namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the <see cref="Service"/> resources in the Content Delivery
    /// Service.
    /// </summary>
    /// <seealso cref="IContentDeliveryService.PrepareListServicesAsync"/>
    /// <seealso cref="ContentDeliveryServiceExtensions.ListServicesAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListServicesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Service>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListServicesApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListServicesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Service>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
