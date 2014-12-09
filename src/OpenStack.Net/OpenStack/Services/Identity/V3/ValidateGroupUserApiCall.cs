namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateGroupUserApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateGroupUserApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
