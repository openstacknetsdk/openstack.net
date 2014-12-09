namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateCredentialApiCall : DelegatingHttpApiCall<Credential>
    {
        public UpdateCredentialApiCall(IHttpApiCall<Credential> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
