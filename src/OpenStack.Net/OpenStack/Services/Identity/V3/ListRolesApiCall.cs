namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListRolesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Role>>
    {
        public ListRolesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Role>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
