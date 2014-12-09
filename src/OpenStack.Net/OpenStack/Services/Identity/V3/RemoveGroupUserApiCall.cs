namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveGroupUserApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveGroupUserApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
