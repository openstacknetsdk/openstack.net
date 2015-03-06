namespace OpenStack.Services.Networking.V2.Quotas
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ResetQuotas</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IQuotasExtension.PrepareResetQuotasAsync"/>
    /// <seealso cref="QuotasExtensions.ResetQuotasAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ResetQuotasApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResetQuotasApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ResetQuotasApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
