namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListGroupUsersApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<User>>
    {
        public ListGroupUsersApiCall(IHttpApiCall<ReadOnlyCollectionPage<User>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
