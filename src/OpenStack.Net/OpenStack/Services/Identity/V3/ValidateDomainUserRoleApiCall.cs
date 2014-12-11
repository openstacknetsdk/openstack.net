namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateDomainUserRoleApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateDomainUserRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
