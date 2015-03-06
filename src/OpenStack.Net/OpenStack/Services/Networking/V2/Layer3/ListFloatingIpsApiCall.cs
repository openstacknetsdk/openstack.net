namespace OpenStack.Services.Networking.V2.Layer3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to list the <see cref="FloatingIp"/> resources with the OpenStack
    /// Networking Service V2.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareListFloatingIpsAsync"/>
    /// <seealso cref="Layer3Extensions.ListFloatingIpsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListFloatingIpsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<FloatingIp>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFloatingIpsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListFloatingIpsApiCall(IHttpApiCall<ReadOnlyCollectionPage<FloatingIp>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
