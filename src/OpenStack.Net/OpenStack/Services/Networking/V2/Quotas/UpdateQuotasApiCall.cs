namespace OpenStack.Services.Networking.V2.Quotas
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>UpdateQuotas</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IQuotasExtension.PrepareUpdateQuotasAsync"/>
    /// <seealso cref="QuotasExtensions.UpdateQuotasAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdateQuotasApiCall : DelegatingHttpApiCall<QuotaResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQuotasApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public UpdateQuotasApiCall(IHttpApiCall<QuotaResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
