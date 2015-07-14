using System.Net.Http;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace OpenStack.Authentication
{
    /// <summary>
    /// Instructs Flurl to use our <see cref="AuthenticatedMessageHandler"/> for all requests.
    /// </summary>
    internal class AuthenticatedHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpClient CreateClient(Url url, HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                Timeout = FlurlHttp.Configuration.DefaultTimeout
            };
        }

        public override HttpMessageHandler CreateMessageHandler()
        {
            return new AuthenticatedMessageHandler(base.CreateMessageHandler());
        }
    }
}