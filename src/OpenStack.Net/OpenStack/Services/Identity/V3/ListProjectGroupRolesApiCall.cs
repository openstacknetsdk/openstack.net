namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListProjectGroupRolesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Role>>
    {
        public ListProjectGroupRolesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Role>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
