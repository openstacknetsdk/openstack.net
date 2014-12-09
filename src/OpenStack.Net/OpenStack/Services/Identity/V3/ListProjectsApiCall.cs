namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListProjectsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Project>>
    {
        public ListProjectsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Project>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
