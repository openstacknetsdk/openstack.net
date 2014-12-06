namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the <see cref="Subnet"/> resources with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="INetworkingService.PrepareListSubnetsAsync"/>
    /// <seealso cref="NetworkingServiceExtensions.ListSubnetsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListSubnetsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Subnet>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListSubnetsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListSubnetsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Subnet>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
