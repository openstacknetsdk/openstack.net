namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddEndpointApiCall : DelegatingHttpApiCall<EndpointResponse>
    {
        public AddEndpointApiCall(IHttpApiCall<EndpointResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
