namespace OpenStack.Services.Networking.V2.Quotas
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>GetQuotas</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IQuotasExtension.PrepareGetQuotasAsync"/>
    /// <seealso cref="QuotasExtensions.GetQuotasAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetQuotasApiCall : DelegatingHttpApiCall<QuotaResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetQuotasApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public GetQuotasApiCall(IHttpApiCall<QuotaResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
