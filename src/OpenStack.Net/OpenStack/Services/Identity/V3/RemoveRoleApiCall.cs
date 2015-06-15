namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveRoleApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveRoleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
