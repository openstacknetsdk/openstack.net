namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetDomainApiCall : DelegatingHttpApiCall<DomainResponse>
    {
        public GetDomainApiCall(IHttpApiCall<DomainResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
