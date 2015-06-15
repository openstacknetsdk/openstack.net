namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateTokenApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateTokenApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
