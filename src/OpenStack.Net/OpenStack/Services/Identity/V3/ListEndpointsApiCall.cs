namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListEndpointsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Endpoint>>
    {
        public ListEndpointsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Endpoint>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
