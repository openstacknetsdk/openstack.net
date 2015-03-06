namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to add a router interface with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareAddRouterInterfaceAsync"/>
    /// <seealso cref="Layer3Extensions.AddRouterInterfaceAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddRouterInterfaceApiCall : DelegatingHttpApiCall<AddRouterInterfaceResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRouterInterfaceApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddRouterInterfaceApiCall(IHttpApiCall<AddRouterInterfaceResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
