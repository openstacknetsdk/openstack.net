namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListVirtualAddresses</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareListVirtualAddressesAsync"/>
    /// <seealso cref="LoadBalancerExtensions.ListVirtualAddressesAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListVirtualAddressesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<VirtualAddress>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListVirtualAddressesApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListVirtualAddressesApiCall(IHttpApiCall<ReadOnlyCollectionPage<VirtualAddress>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
