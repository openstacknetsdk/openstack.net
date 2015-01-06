namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the <see cref="Flavor"/> resources in the Content Delivery
    /// Service.
    /// </summary>
    /// <seealso cref="IContentDeliveryService.PrepareListFlavorsAsync"/>
    /// <seealso cref="ContentDeliveryServiceExtensions.ListFlavorsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListFlavorsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Flavor>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFlavorsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public ListFlavorsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Flavor>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
