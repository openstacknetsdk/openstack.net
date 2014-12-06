namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to remove a <see cref="Router"/> resource with the OpenStack Networking
    /// Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareRemoveRouterAsync"/>
    /// <seealso cref="Layer3Extensions.RemoveRouterAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RemoveRouterApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRouterApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public RemoveRouterApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
