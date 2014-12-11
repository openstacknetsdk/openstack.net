namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveProjectGroupRoleApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveProjectGroupRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
