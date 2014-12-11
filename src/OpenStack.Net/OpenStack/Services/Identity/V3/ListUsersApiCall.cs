namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListUsersApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<User>>
    {
        public ListUsersApiCall(IHttpApiCall<ReadOnlyCollectionPage<User>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
