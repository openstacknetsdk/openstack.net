namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddRoleApiCall : DelegatingHttpApiCall<Role>
    {
        public AddRoleApiCall(IHttpApiCall<Role> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
