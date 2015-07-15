using System.Net.Http;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using OpenStack.Authentication;

namespace OpenStack
{
    /// <summary>
    /// Use this instead of <see cref="Flurl.Http.Testing.HttpTest"/> for any OpenStack.NET unit tests.
    /// <para>
    /// This extends Flurl's default HttpTest to use <see cref="AuthenticatedMessageHandler"/> in unit tests. 
    /// If you use the default HttpTest, then any tests which rely upon authentication handling (e.g retrying a request when a token expires) will fail.
    /// </para>
    /// </summary>
    public class HttpTest : Flurl.Http.Testing.HttpTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTest"/> class.
        /// </summary>
        public HttpTest()
        {
            FlurlHttp.Configure(opts =>
            {
                opts.HttpClientFactory = new TestHttpClientFactory(this);
            });
        }

        class TestHttpClientFactory : IHttpClientFactory
        {
            private readonly Flurl.Http.Testing.TestHttpClientFactory _testMessageHandler;
            private readonly AuthenticatedHttpClientFactory _authenticatedClientFactory;

            public TestHttpClientFactory(HttpTest test)
            {
                _testMessageHandler = new Flurl.Http.Testing.TestHttpClientFactory(test);
                _authenticatedClientFactory = new AuthenticatedHttpClientFactory();
            }

            public HttpClient CreateClient(Url url, HttpMessageHandler handler)
            {
                return _authenticatedClientFactory.CreateClient(url, handler);
            }

            public HttpMessageHandler CreateMessageHandler()
            {
                return new AuthenticatedMessageHandler(_testMessageHandler.CreateMessageHandler());
            }
        }
    }
}