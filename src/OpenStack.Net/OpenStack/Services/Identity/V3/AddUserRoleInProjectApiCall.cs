namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddUserRoleInProjectApiCall : DelegatingHttpApiCall<string>
    {
        public AddUserRoleInProjectApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
