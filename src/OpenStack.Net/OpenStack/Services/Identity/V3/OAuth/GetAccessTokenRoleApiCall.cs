namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class GetAccessTokenRoleApiCall : DelegatingHttpApiCall<RoleResponse>
    {
        public GetAccessTokenRoleApiCall(IHttpApiCall<RoleResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
