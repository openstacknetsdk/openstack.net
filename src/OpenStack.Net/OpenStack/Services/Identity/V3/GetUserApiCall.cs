namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetUserApiCall : DelegatingHttpApiCall<UserResponse>
    {
        public GetUserApiCall(IHttpApiCall<UserResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
