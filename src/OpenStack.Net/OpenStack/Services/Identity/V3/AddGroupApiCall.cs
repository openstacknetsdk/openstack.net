namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddGroupApiCall : DelegatingHttpApiCall<GroupResponse>
    {
        public AddGroupApiCall(IHttpApiCall<GroupResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
