namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListUserRolesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Role>>
    {
        public ListUserRolesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Role>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
