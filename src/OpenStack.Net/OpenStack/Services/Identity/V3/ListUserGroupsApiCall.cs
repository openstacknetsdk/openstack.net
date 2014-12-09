namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListUserGroupsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Group>>
    {
        public ListUserGroupsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Group>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
