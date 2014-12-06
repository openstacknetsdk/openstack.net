namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to get a <see cref="Port"/> resource with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareGetPortAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.GetPortAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetPortApiCall : DelegatingHttpApiCall<PortResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPortApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public GetPortApiCall(IHttpApiCall<PortResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
