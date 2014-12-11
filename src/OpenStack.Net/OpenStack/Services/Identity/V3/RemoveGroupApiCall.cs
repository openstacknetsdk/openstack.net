namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveGroupApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveGroupApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
