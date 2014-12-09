namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveDomainGroupRoleApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveDomainGroupRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
