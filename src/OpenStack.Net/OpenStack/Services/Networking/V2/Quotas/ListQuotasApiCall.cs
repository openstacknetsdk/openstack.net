namespace OpenStack.Services.Networking.V2.Quotas
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListQuotas</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IQuotasExtension.PrepareListQuotasAsync"/>
    /// <seealso cref="QuotasExtensions.ListQuotasAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListQuotasApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Quota>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListQuotasApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListQuotasApiCall(IHttpApiCall<ReadOnlyCollectionPage<Quota>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
