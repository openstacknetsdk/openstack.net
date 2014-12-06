namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to remove a router interface with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareRemoveRouterInterfaceAsync"/>
    /// <seealso cref="Layer3Extensions.RemoveRouterInterfaceAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RemoveRouterInterfaceApiCall : DelegatingHttpApiCall<RemoveRouterInterfaceResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRouterInterfaceApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public RemoveRouterInterfaceApiCall(IHttpApiCall<RemoveRouterInterfaceResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
