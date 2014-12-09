namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveUserRoleInProjectApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveUserRoleInProjectApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
