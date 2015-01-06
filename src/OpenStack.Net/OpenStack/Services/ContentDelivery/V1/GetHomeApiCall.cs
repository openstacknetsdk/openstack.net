namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using OpenStack.Net;
    using OpenStack.ObjectModel.JsonHome;

    /// <summary>
    /// This class represents an HTTP API call to get a JSON Home Document describing the operations supported by the
    /// Content Delivery Service.
    /// </summary>
    /// <seealso cref="IContentDeliveryService.PrepareGetHomeAsync"/>
    /// <seealso cref="ContentDeliveryServiceExtensions.GetHomeAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetHomeApiCall : DelegatingHttpApiCall<HomeDocument>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHomeApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetHomeApiCall(IHttpApiCall<HomeDocument> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
