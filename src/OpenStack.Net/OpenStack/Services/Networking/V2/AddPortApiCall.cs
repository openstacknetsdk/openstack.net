namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to add a <see cref="Port"/> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareAddPortAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.AddPortAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddPortApiCall : DelegatingHttpApiCall<PortResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddPortApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddPortApiCall(IHttpApiCall<PortResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
