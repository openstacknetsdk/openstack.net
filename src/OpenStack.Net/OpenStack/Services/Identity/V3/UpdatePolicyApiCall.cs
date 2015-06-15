namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdatePolicyApiCall : DelegatingHttpApiCall<Policy>
    {
        public UpdatePolicyApiCall(IHttpApiCall<Policy> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
