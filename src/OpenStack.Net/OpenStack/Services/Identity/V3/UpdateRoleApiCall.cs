namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateRoleApiCall : DelegatingHttpApiCall<RoleResponse>
    {
        public UpdateRoleApiCall(IHttpApiCall<RoleResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
