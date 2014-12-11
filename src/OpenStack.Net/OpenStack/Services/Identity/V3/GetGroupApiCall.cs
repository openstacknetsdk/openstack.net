namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetGroupApiCall : DelegatingHttpApiCall<GroupResponse>
    {
        public GetGroupApiCall(IHttpApiCall<GroupResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
