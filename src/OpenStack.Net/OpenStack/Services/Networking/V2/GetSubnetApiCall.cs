namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to get a <see cref="Subnet"/> resource with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareGetSubnetAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.GetSubnetAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetSubnetApiCall : DelegatingHttpApiCall<SubnetResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSubnetApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public GetSubnetApiCall(IHttpApiCall<SubnetResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
