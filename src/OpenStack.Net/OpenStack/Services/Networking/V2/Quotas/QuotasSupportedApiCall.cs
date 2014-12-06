namespace OpenStack.Services.Networking.V2.Quotas
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Services.Identity.V2;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>QuotasSupported</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IQuotasExtension.PrepareQuotasSupportedAsync"/>
    /// <seealso cref="QuotasExtensions.SupportsQuotasAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class QuotasSupportedApiCall : DelegatingHttpApiCall<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuotasSupportedApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public QuotasSupportedApiCall(IHttpApiCall<bool> httpApiCall)
            : base(httpApiCall)
        {
        }

        public QuotasSupportedApiCall(ListExtensionsApiCall httpApiCall)
            : this(WrapListExtensionsCall(httpApiCall))
        {
        }

        protected static bool SelectResult(HttpResponseMessage response, ReadOnlyCollection<Extension> result, CancellationToken cancellationToken)
        {
            return result != null && result.Any(i => QuotasExtension.ExtensionAlias == i.Alias);
        }

        protected static IHttpApiCall<bool> WrapListExtensionsCall(ListExtensionsApiCall httpApiCall)
        {
            return new TransformHttpApiCall<ReadOnlyCollectionPage<Extension>, bool>(httpApiCall, SelectResult);
        }
    }
}
