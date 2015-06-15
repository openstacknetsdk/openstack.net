namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveProjectUserRoleApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveProjectUserRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
