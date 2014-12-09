namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateServiceApiCall : DelegatingHttpApiCall<ServiceResponse>
    {
        public UpdateServiceApiCall(IHttpApiCall<ServiceResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
