namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>UpdateNetwork</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareUpdateNetworkAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.UpdateNetworkAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdateNetworkApiCall : DelegatingHttpApiCall<NetworkResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateNetworkApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public UpdateNetworkApiCall(IHttpApiCall<NetworkResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
