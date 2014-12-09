namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class ValidateUserRoleInProjectApiCall : DelegatingHttpApiCall<string>
    {
        public ValidateUserRoleInProjectApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
