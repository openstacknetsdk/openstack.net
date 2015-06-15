namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListConsumersApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Consumer>>
    {
        public ListConsumersApiCall(IHttpApiCall<ReadOnlyCollectionPage<Consumer>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
