namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the <see cref="Port"/> resources with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareListPortsAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.ListPortsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListPortsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Port>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListPortsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListPortsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Port>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
