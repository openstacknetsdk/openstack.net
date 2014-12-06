namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to add a <see cref="VirtualAddress"/> resource with the OpenStack
    /// Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareAddVirtualAddressAsync"/>
    /// <seealso cref="LoadBalancerExtensions.AddVirtualAddressAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddVirtualAddressApiCall : DelegatingHttpApiCall<VirtualAddressResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddVirtualAddressApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddVirtualAddressApiCall(IHttpApiCall<VirtualAddressResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
