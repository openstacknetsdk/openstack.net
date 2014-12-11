namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateProjectUserRoleApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateProjectUserRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
