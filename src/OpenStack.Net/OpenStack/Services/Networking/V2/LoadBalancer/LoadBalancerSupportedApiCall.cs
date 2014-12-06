namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Services.Identity.V2;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>LoadBalancerSupported</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareLoadBalancerSupportedAsync"/>
    /// <seealso cref="LoadBalancerExtensions.SupportsLoadBalancerAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class LoadBalancerSupportedApiCall : DelegatingHttpApiCall<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerSupportedApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public LoadBalancerSupportedApiCall(IHttpApiCall<bool> httpApiCall)
            : base(httpApiCall)
        {
        }

        public LoadBalancerSupportedApiCall(ListExtensionsApiCall httpApiCall)
            : this(WrapListExtensionsCall(httpApiCall))
        {
        }

        protected static bool SelectResult(HttpResponseMessage response, ReadOnlyCollection<Extension> result, CancellationToken cancellationToken)
        {
            return result != null && result.Any(i => LoadBalancerExtension.ExtensionAlias == i.Alias);
        }

        protected static IHttpApiCall<bool> WrapListExtensionsCall(ListExtensionsApiCall httpApiCall)
        {
            return new TransformHttpApiCall<ReadOnlyCollectionPage<Extension>, bool>(httpApiCall, SelectResult);
        }
    }
}
