namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemovePolicyApiCall : DelegatingHttpApiCall<string>
    {
        public RemovePolicyApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
