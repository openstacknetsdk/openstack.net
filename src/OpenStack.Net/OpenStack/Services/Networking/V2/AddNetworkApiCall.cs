namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to add a <see cref="Network"/> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareAddNetworkAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.AddNetworkAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddNetworkApiCall : DelegatingHttpApiCall<NetworkResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNetworkApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddNetworkApiCall(IHttpApiCall<NetworkResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
