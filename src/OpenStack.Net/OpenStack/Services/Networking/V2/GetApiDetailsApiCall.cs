namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to get information about a specific <seealso cref="ApiVersion"/> with the
    /// OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareGetApiDetailsAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.GetApiDetailsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetApiDetailsApiCall : DelegatingHttpApiCall<ApiDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetApiDetailsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public GetApiDetailsApiCall(IHttpApiCall<ApiDetails> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
