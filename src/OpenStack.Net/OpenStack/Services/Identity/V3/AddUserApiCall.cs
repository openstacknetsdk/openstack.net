namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddUserApiCall : DelegatingHttpApiCall<UserResponse>
    {
        public AddUserApiCall(IHttpApiCall<UserResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
