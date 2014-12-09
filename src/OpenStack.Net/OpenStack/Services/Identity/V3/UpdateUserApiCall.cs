namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateUserApiCall : DelegatingHttpApiCall<UserResponse>
    {
        public UpdateUserApiCall(IHttpApiCall<UserResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
