namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetRoleApiCall : DelegatingHttpApiCall<RoleResponse>
    {
        public GetRoleApiCall(IHttpApiCall<RoleResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
