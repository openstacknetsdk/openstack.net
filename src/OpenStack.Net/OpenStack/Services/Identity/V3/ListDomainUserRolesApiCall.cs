namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListDomainUserRolesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Role>>
    {
        public ListDomainUserRolesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Role>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
