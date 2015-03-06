namespace OpenStack.Services.Networking
{
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Services.Networking.V2;

    /// <summary>
    /// This class represents an HTTP API call to list the API versions exposed by the OpenStack Networking Service.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareListApiVersionsAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.ListApiVersionsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListApiVersionsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<ApiVersion>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListApiVersionsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListApiVersionsApiCall(IHttpApiCall<ReadOnlyCollectionPage<ApiVersion>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
