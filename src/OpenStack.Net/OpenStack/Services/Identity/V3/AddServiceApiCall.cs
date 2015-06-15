namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddServiceApiCall : DelegatingHttpApiCall<ServiceResponse>
    {
        public AddServiceApiCall(IHttpApiCall<ServiceResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
