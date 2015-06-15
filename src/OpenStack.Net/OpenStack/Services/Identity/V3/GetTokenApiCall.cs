namespace OpenStack.Services.Identity.V3
{
    using System;
    using OpenStack.Net;

    public class GetTokenApiCall : DelegatingHttpApiCall<Tuple<TokenId, AuthenticateResponse>>
    {
        public GetTokenApiCall(IHttpApiCall<Tuple<TokenId, AuthenticateResponse>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
