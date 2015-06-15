namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddProjectApiCall : DelegatingHttpApiCall<ProjectResponse>
    {
        public AddProjectApiCall(IHttpApiCall<ProjectResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
