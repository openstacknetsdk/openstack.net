namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateEndpointApiCall : DelegatingHttpApiCall<EndpointResponse>
    {
        public UpdateEndpointApiCall(IHttpApiCall<EndpointResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
