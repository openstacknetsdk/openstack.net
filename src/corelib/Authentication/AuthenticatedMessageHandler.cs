using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace OpenStack.Authentication
{
    /// <summary>
    /// Used by Flurl for all requests. Understands how to authenticate and retry when a token expires.
    /// </summary>
    /// <exclude />
    internal class AuthenticatedMessageHandler : FlurlMessageHandler
    {
        public AuthenticatedMessageHandler(HttpMessageHandler innerHandler) 
            : base(innerHandler)
        { }

        public IAuthenticationProvider AuthenticationProvider;

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = await AuthenticationProvider.GetToken(cancellationToken).ConfigureAwait(false);
            request.Headers.SetAuthToken(token);

            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus != HttpStatusCode.Unauthorized)
                    throw;
            }
                
            // Retry with a new token
            var retryRequest = request.Copy();
            var retryToken = await AuthenticationProvider.GetToken(cancellationToken).ConfigureAwait(false);
            retryRequest.Headers.SetAuthToken(retryToken);
            return await base.SendAsync(retryRequest, cancellationToken).ConfigureAwait(false);
        }
    }
}