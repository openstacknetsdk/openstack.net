namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveTokenApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveTokenApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
