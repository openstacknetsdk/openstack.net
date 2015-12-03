using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.Net.Http.Headers;
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
        /// <summary>
        /// Global configuration which affects OpenStack.NET's behavior.
        /// <para>Modify using <see cref="Configure"/>.</para>
        /// </summary>
        public static readonly OpenStackNetConfigurationOptions Configuration = new OpenStackNetConfigurationOptions();
        private static readonly object ConfigureLock = new object();
        private static bool _isConfigured;

        /// <summary>
        /// Provides thread-safe accesss to OpenStack.NET's global configuration options.
        /// <para>
        /// Can only be called once at application start-up, before instantiating any OpenStack.NET objects.
        /// </para>
        /// </summary>
        /// <param name="configureFlurl">Addtional configuration of Flurl's global settings <seealso cref="Flurl.Http.FlurlHttp.Configure" />.</param>
        /// <param name="configureJson">Additional configuration of Json.NET's global settings <seealso cref="Newtonsoft.Json.JsonConvert.DefaultSettings" />.</param>
        /// <param name="configure">Additional configuration of OpenStack.NET's global settings.</param>
        public static void Configure(Action<FlurlHttpConfigurationOptions> configureFlurl = null, Action<JsonSerializerSettings> configureJson = null, Action<OpenStackNetConfigurationOptions> configure = null)
        {
            lock (ConfigureLock)
            {
                if (_isConfigured)
                    return;

                if(configure != null)
                    configure(Configuration);

                ConfigureJson(configureJson);
                ConfigureFlurl(configureFlurl);

                _isConfigured = true;
            }
        }

        /// <summary>
        /// Resets all configuration (OpenStack.NET, Flurl and Json.NET) so that <see cref="Configure"/> can be called again.
        /// </summary>
        public static void ResetDefaults()
        {
            lock (ConfigureLock)
            {
                Configuration.ResetDefaults();
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings();
                FlurlHttp.Configuration.ResetDefaults();

                _isConfigured = false;
            }
        }

        private static void ConfigureJson(Action<JsonSerializerSettings> configureJson = null)
        {
            JsonConvert.DefaultSettings = () =>
            {
                // Apply our default settings
                var settings = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new OpenStackContractResolver()
                };

                // Apply application's default settings
                if (configureJson != null)
                    configureJson(settings);
                return settings;
            };
        }

        private static void ConfigureFlurl(Action<FlurlHttpConfigurationOptions> configureFlurl = null)
        {
            FlurlHttp.Configure(c =>
            {
                // Apply the application's default settings
                if (configureFlurl != null)
                    configureFlurl(c);

                //
                // Apply our default settings
                //
                if (c.HttpClientFactory is DefaultHttpClientFactory)
                    c.HttpClientFactory = new AuthenticatedHttpClientFactory();

                // Apply our event handling without clobbering any application level handlers
                var applicationBeforeCall = c.BeforeCall;
                c.BeforeCall = call =>
                {
                    SetUserAgentHeader(call);
                    if (applicationBeforeCall != null)
                        applicationBeforeCall(call);
                };

                var applicationAfterCall = c.AfterCall;
                c.AfterCall = call =>
                {
                    Tracing.TraceHttpCall(call);
                    if (applicationAfterCall != null)
                        applicationAfterCall(call);
                };

                var applicationOnError = c.OnError;
                c.OnError = call =>
                {
                    Tracing.TraceFailedHttpCall(call);
                    if (applicationOnError != null)
                        applicationOnError(call);
                };
            });
        }

        private static void SetUserAgentHeader(HttpCall call)
        {
            foreach (var userAgent in Configuration.UserAgents)
            {
                call.Request.Headers.UserAgent.Add(userAgent);
            }
        }

        /// <summary>
        /// Provides global point for programmatically configuraing tracing
        /// </summary>
        public static class Tracing
        {
            /// <summary>
            /// Trace source for all HTTP requests. Default level is Error.
            /// <para>
            /// In your app or web.config the trace soruce name is "Flurl.Http".
            /// </para>
            /// </summary>
            public static readonly TraceSource Http = new TraceSource("Flurl.Http", SourceLevels.Error);

            /// <summary>
            /// Traces a failed HTTP request
            /// </summary>
            /// <param name="httpCall">The Flurl HTTP call instance, containing information about the request and response.</param>
            public static void TraceFailedHttpCall(HttpCall httpCall)
            {
                Http.TraceData(TraceEventType.Error, 0, JsonConvert.SerializeObject(httpCall, Formatting.Indented));
                Http.Flush();
            }

            /// <summary>
            /// Traces an HTTP request
            /// </summary>
            /// <param name="httpCall">The Flurl HTTP call instance, containing information about the request and response.</param>
            public static void TraceHttpCall(HttpCall httpCall)
            {
                Http.TraceData(TraceEventType.Information, 0, JsonConvert.SerializeObject(httpCall, Formatting.Indented));
            }
        }
    }

    /// <summary>
    /// A set of properties that affect OpenStack.NET's behavior.
    /// <para>Generally set via the static <see cref="OpenStack.OpenStackNet.Configure"/> method.</para>
    /// </summary>
    public class OpenStackNetConfigurationOptions
    {
        /// <summary/>
        protected internal OpenStackNetConfigurationOptions()
        {
            ResetDefaults();
        }

        /// <summary> 
        /// Additional application specific user agents which should be set in the UserAgent header on all requests.
        /// </summary>
        public List<ProductInfoHeaderValue> UserAgents { get; private set; }

        /// <summary>
		/// Clear all custom global options and set default values.
		/// </summary>
        public virtual void ResetDefaults()
        {
            UserAgents = new List<ProductInfoHeaderValue>
            {
                new ProductInfoHeaderValue("openstack.net", GetType().GetAssemblyFileVersion())
            };
        }
    }
}
