namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListUserProjectsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Project>>
    {
        public ListUserProjectsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Project>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
