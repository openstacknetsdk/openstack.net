namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class GetAccessTokenApiCall : DelegatingHttpApiCall<AccessTokenResponse>
    {
        public GetAccessTokenApiCall(IHttpApiCall<AccessTokenResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
