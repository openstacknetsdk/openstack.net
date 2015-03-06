namespace OpenStack.Services.Networking.V2.Metering
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Services.Identity.V2;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>MeteringSupported</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IMeteringExtension.PrepareMeteringSupportedAsync"/>
    /// <seealso cref="MeteringExtensions.SupportsMeteringAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class MeteringSupportedApiCall : DelegatingHttpApiCall<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringSupportedApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public MeteringSupportedApiCall(IHttpApiCall<bool> httpApiCall)
            : base(httpApiCall)
        {
        }

        public MeteringSupportedApiCall(ListExtensionsApiCall httpApiCall)
            : this(WrapListExtensionsCall(httpApiCall))
        {
        }

        protected static bool SelectResult(HttpResponseMessage response, ReadOnlyCollection<Extension> result, CancellationToken cancellationToken)
        {
            return result != null && result.Any(i => MeteringExtension.ExtensionAlias == i.Alias);
        }

        protected static IHttpApiCall<bool> WrapListExtensionsCall(ListExtensionsApiCall httpApiCall)
        {
            return new TransformHttpApiCall<ReadOnlyCollectionPage<Extension>, bool>(httpApiCall, SelectResult);
        }
    }
}
