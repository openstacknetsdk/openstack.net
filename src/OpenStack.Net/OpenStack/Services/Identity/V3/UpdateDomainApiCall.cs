namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateDomainApiCall : DelegatingHttpApiCall<DomainResponse>
    {
        public UpdateDomainApiCall(IHttpApiCall<DomainResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
