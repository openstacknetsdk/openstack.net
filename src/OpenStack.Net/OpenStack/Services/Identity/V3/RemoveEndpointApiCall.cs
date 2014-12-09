namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveEndpointApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveEndpointApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
