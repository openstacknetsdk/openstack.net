namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveDomainApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveDomainApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
