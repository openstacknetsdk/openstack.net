namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddProjectUserRoleApiCall : DelegatingHttpApiCall<string>
    {
        public AddProjectUserRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
