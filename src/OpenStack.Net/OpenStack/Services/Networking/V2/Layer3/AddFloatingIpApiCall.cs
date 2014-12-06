namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to add a floating IP address with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareAddFloatingIpAsync"/>
    /// <seealso cref="Layer3Extensions.AddFloatingIpAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddFloatingIpApiCall : DelegatingHttpApiCall<FloatingIpResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddFloatingIpApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddFloatingIpApiCall(IHttpApiCall<FloatingIpResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
