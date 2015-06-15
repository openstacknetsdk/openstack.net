namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveCredentialApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveCredentialApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
