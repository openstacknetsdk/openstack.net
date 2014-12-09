namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddCredentialApiCall : DelegatingHttpApiCall<Credential>
    {
        public AddCredentialApiCall(IHttpApiCall<Credential> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
