using System;
using System.Diagnostics;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using OpenStack.Authentication;
using OpenStack.Serialization;

namespace OpenStack
{
    /// <summary>
    /// A static container for global configuration settings affecting OpenStack.NET behavior.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public static class OpenStackNet
    {
        private static readonly object ConfigureLock = new object();
        private static bool _isConfigured = false;

        /// <summary>
        /// Provides thread-safe accesss to OpenStack.NET's global configuration options.
        /// <para>
        /// Can only be called once at application start-up, before instantiating any OpenStack.NET objects.
        /// </para>
        /// </summary>
        /// <param name="configureFlurl">Addtional configuration of Flurl's global settings <seealso cref="Flurl.Http.FlurlHttp.Configure"/>.</param>
        /// <param name="configureJson">Additional configuration of Json.NET's glboal settings <seealso cref="Newtonsoft.Json.JsonConvert.DefaultSettings"/>.</param>
        public static void Configure(Action<FlurlHttpConfigurationOptions> configureFlurl = null, Action<JsonSerializerSettings> configureJson = null)
        {
            if (_isConfigured)
                return;

            lock (ConfigureLock)
            {
                if (_isConfigured)
                    return;

                JsonConvert.DefaultSettings = () =>
                {
                    // Apply our default settings
                    var settings = new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new EmptyEnumerableResolver()
                    };

                    // Apply application's default settings
                    if (configureJson != null)
                        configureJson(settings);
                    return settings;
                };

                FlurlHttp.Configure(c =>
                {
                    // Apply our default settings
                    c.HttpClientFactory = new AuthenticatedHttpClientFactory();
                    c.AfterCall = Tracing.TraceHttpCall;
                    c.OnError = Tracing.TraceFailedHttpCall;

                    // Apply application's default settings
                    if (configureFlurl != null)
                        configureFlurl(c);
                });

                _isConfigured = true;
            }
        }

        public static class Tracing
        {
            public static readonly TraceSource Http = new TraceSource("Flurl.Http");

            public static void TraceFailedHttpCall(HttpCall httpCall)
            {
                Http.TraceData(TraceEventType.Error, 0, JsonConvert.SerializeObject(httpCall, Formatting.Indented));
                Http.Flush();
            }

            public static void TraceHttpCall(HttpCall httpCall)
            {
                Http.TraceData(TraceEventType.Information, 0, JsonConvert.SerializeObject(httpCall, Formatting.Indented));
            }
        }
    }
}
