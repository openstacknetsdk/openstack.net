namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveUserApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveUserApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
