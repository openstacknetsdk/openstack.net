namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddDomainGroupRoleApiCall : DelegatingHttpApiCall<string>
    {
        public AddDomainGroupRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
