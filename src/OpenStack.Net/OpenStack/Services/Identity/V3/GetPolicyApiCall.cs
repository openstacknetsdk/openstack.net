namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetPolicyApiCall : DelegatingHttpApiCall<Policy>
    {
        public GetPolicyApiCall(IHttpApiCall<Policy> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
