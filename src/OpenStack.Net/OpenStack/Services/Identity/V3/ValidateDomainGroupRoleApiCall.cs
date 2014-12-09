namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateDomainGroupRoleApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateDomainGroupRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
