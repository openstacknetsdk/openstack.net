namespace OpenStack.Services.Networking.V2.Layer3
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Services.Identity.V2;

    /// <summary>
    /// This class represents an HTTP API call to determine whether or not the OpenStack Networking Service V2 supports
    /// the <see cref="PredefinedNetworkingExtensions.Layer3"/> extension.
    /// </summary>
    /// <seealso cref="ILayer3Extension.PrepareLayer3SupportedAsync"/>
    /// <seealso cref="Layer3Extensions.SupportsLayer3Async"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class Layer3SupportedApiCall : DelegatingHttpApiCall<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Layer3SupportedApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public Layer3SupportedApiCall(IHttpApiCall<bool> httpApiCall)
            : base(httpApiCall)
        {
        }

        public Layer3SupportedApiCall(ListExtensionsApiCall httpApiCall)
            : this(WrapListExtensionsCall(httpApiCall))
        {
        }

        protected static bool SelectResult(HttpResponseMessage response, ReadOnlyCollection<Extension> result, CancellationToken cancellationToken)
        {
            return result != null && result.Any(i => Layer3Extension.ExtensionAlias == i.Alias);
        }

        protected static IHttpApiCall<bool> WrapListExtensionsCall(ListExtensionsApiCall httpApiCall)
        {
            return new TransformHttpApiCall<ReadOnlyCollectionPage<Extension>, bool>(httpApiCall, SelectResult);
        }
    }
}
