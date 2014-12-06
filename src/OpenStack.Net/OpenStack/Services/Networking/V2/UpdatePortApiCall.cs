namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>UpdatePort</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareUpdatePortAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.UpdatePortAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdatePortApiCall : DelegatingHttpApiCall<PortResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePortApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public UpdatePortApiCall(IHttpApiCall<PortResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
