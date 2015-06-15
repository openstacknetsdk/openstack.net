namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListPoliciesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Policy>>
    {
        public ListPoliciesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Policy>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
