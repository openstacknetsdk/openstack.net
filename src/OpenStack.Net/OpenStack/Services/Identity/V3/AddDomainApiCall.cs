namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddDomainApiCall : DelegatingHttpApiCall<DomainResponse>
    {
        public AddDomainApiCall(IHttpApiCall<DomainResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
