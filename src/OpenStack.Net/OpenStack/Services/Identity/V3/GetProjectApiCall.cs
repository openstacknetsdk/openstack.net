namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class GetProjectApiCall : DelegatingHttpApiCall<ProjectResponse>
    {
        public GetProjectApiCall(IHttpApiCall<ProjectResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
