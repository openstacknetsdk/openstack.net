namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetEndpointApiCall : DelegatingHttpApiCall<EndpointResponse>
    {
        public GetEndpointApiCall(IHttpApiCall<EndpointResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
