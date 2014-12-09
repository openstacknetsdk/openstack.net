namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddRoleToUserApiCall : DelegatingHttpApiCall<string>
    {
        public AddRoleToUserApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
