namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListAccessTokenRolesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Role>>
    {
        public ListAccessTokenRolesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Role>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
