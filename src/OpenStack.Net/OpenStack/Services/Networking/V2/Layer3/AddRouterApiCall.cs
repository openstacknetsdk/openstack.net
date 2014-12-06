namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to get a <see cref="Port"/> resource with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareAddRouterAsync"/>
    /// <seealso cref="Layer3Extensions.AddRouterAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddRouterApiCall : DelegatingHttpApiCall<RouterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRouterApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddRouterApiCall(IHttpApiCall<RouterResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
