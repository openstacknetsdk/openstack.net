namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetServiceApiCall : DelegatingHttpApiCall<ServiceResponse>
    {
        public GetServiceApiCall(IHttpApiCall<ServiceResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
