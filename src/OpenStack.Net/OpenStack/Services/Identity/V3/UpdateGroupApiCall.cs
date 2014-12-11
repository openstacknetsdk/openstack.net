namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateGroupApiCall : DelegatingHttpApiCall<GroupResponse>
    {
        public UpdateGroupApiCall(IHttpApiCall<GroupResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
