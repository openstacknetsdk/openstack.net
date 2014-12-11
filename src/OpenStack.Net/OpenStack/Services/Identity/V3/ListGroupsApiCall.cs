namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListGroupsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Group>>
    {
        public ListGroupsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Group>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
