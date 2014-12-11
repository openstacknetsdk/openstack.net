namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListProjectUserRolesApiCall: DelegatingHttpApiCall<ReadOnlyCollectionPage<Role>>
    {
        public ListProjectUserRolesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Role>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
