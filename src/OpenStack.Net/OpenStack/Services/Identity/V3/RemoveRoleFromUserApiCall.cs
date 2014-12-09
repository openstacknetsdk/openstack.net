namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveRoleFromUserApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveRoleFromUserApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
