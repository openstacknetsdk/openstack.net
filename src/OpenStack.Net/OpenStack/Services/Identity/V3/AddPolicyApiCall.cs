namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddPolicyApiCall : DelegatingHttpApiCall<Policy>
    {
        public AddPolicyApiCall(IHttpApiCall<Policy> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
