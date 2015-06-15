namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddProjectGroupRoleApiCall : DelegatingHttpApiCall<string>
    {
        public AddProjectGroupRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
