using System;
using System.Net;
using System.Net.Http;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Http.Content;
using OpenStack.Authentication;

namespace OpenStack.Testing
{
    /// <summary>
    /// Use this instead of <see cref="Flurl.Http.Testing.HttpTest"/> for any OpenStack.NET unit tests.
    /// <para>
    /// This extends Flurl's default HttpTest to use <see cref="AuthenticatedMessageHandler"/> in unit tests. 
    /// If you use the default HttpTest, then any tests which rely upon authentication handling (e.g retrying a request when a token expires) will fail.
    /// </para>
    /// </summary>
    public class HttpTest : Flurl.Http.Testing.HttpTest, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTest"/> class.
        /// </summary>
        /// <param name="configureFlurl">Addtional configuration of the OpenStack.NET Flurl client settings <seealso cref="Flurl.Http.FlurlHttp.Configure" />.</param>
        /// <param name="configure">Additional configuration of OpenStack.NET's global settings.</param>
        public HttpTest(Action<FlurlHttpSettings> configureFlurl = null, Action<OpenStackNetConfigurationOptions> configure = null)
        {
            Action<FlurlHttpSettings> setFlurlTestMode = opts =>
            {
                configureFlurl?.Invoke(opts);
                opts.HttpClientFactory = new TestHttpClientFactory(this);
                opts.AfterCall = call =>
                {
                    CallLog.Add(call);
                };
            };
            OpenStackNet.ResetDefaults();
            OpenStackNet.Configure(setFlurlTestMode, configure);
        }

        /// <inheritdoc />
        public new void Dispose()
        {
            OpenStackNet.ResetDefaults();
            base.Dispose();
        }

        /// <inheritdoc />
        public new HttpTest RespondWithJson(object data)
        {
            return RespondWithJson(200, data);
        }

        /// <inheritdoc />
        public new HttpTest RespondWithJson(int status, object data)
        {
            ResponseQueue.Enqueue(new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)status,
                Content = new CapturedJsonContent(OpenStackNet.Configuration.FlurlHttpSettings.JsonSerializer.Serialize(data))
            });
            return this;
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