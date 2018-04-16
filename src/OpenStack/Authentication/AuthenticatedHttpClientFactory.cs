using System.Net.Http;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace OpenStack.Authentication
{
    /// <summary>
    /// Instructs Flurl to use an <see cref="AuthenticatedMessageHandler"/> for all requests.
    /// </summary>
    /// <exclude />
    public class AuthenticatedHttpClientFactory : DefaultHttpClientFactory
    {
        /// <inheritdoc/>
        public override HttpClient CreateClient(Url url, HttpMessageHandler handler)
        {
            return new HttpClient(handler)
            {
                Timeout = FlurlHttp.GlobalSettings.DefaultTimeout
            };
        }

        /// <inheritdoc/>
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new AuthenticatedMessageHandler(base.CreateMessageHandler());
        }
    }
}