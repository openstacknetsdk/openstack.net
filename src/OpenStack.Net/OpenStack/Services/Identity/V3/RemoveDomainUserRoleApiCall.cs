namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveDomainUserRoleApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveDomainUserRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
