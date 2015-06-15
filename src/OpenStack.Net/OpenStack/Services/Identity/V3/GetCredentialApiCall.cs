namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetCredentialApiCall : DelegatingHttpApiCall<Credential>
    {
        public GetCredentialApiCall(IHttpApiCall<Credential> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
