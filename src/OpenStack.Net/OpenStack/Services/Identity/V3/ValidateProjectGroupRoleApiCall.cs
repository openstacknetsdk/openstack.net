namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateProjectGroupRoleApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateProjectGroupRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
