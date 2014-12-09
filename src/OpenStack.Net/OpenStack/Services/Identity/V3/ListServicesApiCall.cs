namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListServicesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Service>>
    {
        public ListServicesApiCall(IHttpApiCall<ReadOnlyCollectionPage<Service>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
