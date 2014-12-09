namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateRoleApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
