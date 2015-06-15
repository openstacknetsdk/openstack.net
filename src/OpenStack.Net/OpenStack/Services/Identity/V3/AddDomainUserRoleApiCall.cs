namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddDomainUserRoleApiCall : DelegatingHttpApiCall<string>
    {
        public AddDomainUserRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
