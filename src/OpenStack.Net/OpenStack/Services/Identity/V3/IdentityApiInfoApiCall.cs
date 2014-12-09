namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class IdentityApiInfoApiCall : DelegatingHttpApiCall<ApiVersionResponse>
    {
        public IdentityApiInfoApiCall(IHttpApiCall<ApiVersionResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
