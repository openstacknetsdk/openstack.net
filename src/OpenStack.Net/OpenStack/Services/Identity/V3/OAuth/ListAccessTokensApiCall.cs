namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Collections;
    using OpenStack.Net;

    public class ListAccessTokensApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<AccessToken>>
    {
        public ListAccessTokensApiCall(IHttpApiCall<ReadOnlyCollectionPage<AccessToken>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
