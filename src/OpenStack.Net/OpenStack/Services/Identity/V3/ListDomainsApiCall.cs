namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListDomainsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Domain>>
    {
        public ListDomainsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Domain>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
