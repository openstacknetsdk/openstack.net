namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to update a <see cref="Router"/> resource with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareUpdateRouterAsync"/>
    /// <seealso cref="Layer3Extensions.UpdateRouterAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdateRouterApiCall : DelegatingHttpApiCall<RouterResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRouterApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public UpdateRouterApiCall(IHttpApiCall<RouterResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
