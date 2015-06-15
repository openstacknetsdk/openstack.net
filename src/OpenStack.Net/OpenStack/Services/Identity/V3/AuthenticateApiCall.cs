namespace OpenStack.Services.Identity.V3
{
    using System;
    using OpenStack.Net;

    public class AuthenticateApiCall : DelegatingHttpApiCall<Tuple<TokenId, AuthenticateResponse>>
    {
        public AuthenticateApiCall(IHttpApiCall<Tuple<TokenId, AuthenticateResponse>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
