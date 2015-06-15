namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class RemoveAccessTokenApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveAccessTokenApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
