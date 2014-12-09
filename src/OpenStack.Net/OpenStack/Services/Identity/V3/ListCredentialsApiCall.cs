namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListCredentialsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Credential>>
    {
        public ListCredentialsApiCall(IHttpApiCall<ReadOnlyCollectionPage<Credential>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
