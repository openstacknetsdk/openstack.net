namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveProjectApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveProjectApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
