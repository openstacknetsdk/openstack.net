namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class UpdateProjectApiCall : DelegatingHttpApiCall<ProjectResponse>
    {
        public UpdateProjectApiCall(IHttpApiCall<ProjectResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
