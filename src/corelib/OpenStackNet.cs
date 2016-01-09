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
        public static OpenStackNetConfigurationOptions Configuration
        {
            get
            {
                if (!_isConfigured)
                    Configure();
                return _config;
            }
        }

        private static readonly OpenStackNetConfigurationOptions _config = new OpenStackNetConfigurationOptions();
        private static readonly object ConfigureLock = new object();
        private static bool _isConfigured;

        /// <summary>
        /// Provides thread-safe accesss to OpenStack.NET's global configuration options.
        /// <para>
        /// Can only be called once at application start-up, before instantiating any OpenStack.NET objects.
        /// </para>
        /// </summary>
        /// <param name="configureFlurl">Addtional configuration of the OpenStack.NET Flurl client settings <seealso cref="Flurl.Http.FlurlHttp.Configure" />.</param>
        /// <param name="configure">Additional configuration of OpenStack.NET's global settings.</param>
        public static void Configure(Action<FlurlHttpSettings> configureFlurl = null, Action<OpenStackNetConfigurationOptions> configure = null)
        {
            lock (ConfigureLock)
            {
                // Check if a user is attempting to apply custom configuration after the default config has been applied
                if (_isConfigured && (configureFlurl != null || configure != null))
                {
                    Trace.TraceError("Ignoring additional call to OpenStackNet.Configure. It can only be called once at application start-up, before instantiating any OpenStack.NET objects.");
                    return;
                }

                configure?.Invoke(_config);
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
                _config.ResetDefaults();
                _isConfigured = false;
            }
        }

        private static void ConfigureFlurl(Action<FlurlHttpSettings> configureFlurl = null)
        {
            var flurlSettings = _config.FlurlHttpSettings;

            // Apply the application's default settings
            configureFlurl?.Invoke(flurlSettings);

            flurlSettings.JsonSerializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new OpenStackContractResolver()
            });

            //
            // Apply our default settings
            //
            if (flurlSettings.HttpClientFactory is DefaultHttpClientFactory)
                flurlSettings.HttpClientFactory = new AuthenticatedHttpClientFactory();

            // Apply our event handling without clobbering any application level handlers
            var applicationBeforeCall = flurlSettings.BeforeCall;
            flurlSettings.BeforeCall = call =>
            {
                SetUserAgentHeader(call);
                applicationBeforeCall?.Invoke(call);
            };

            var applicationAfterCall = flurlSettings.AfterCall;
            flurlSettings.AfterCall = call =>
            {
                Tracing.TraceHttpCall(call);
                applicationAfterCall?.Invoke(call);
            };

            var applicationOnError = flurlSettings.OnError;
            flurlSettings.OnError = call =>
            {
                Tracing.TraceFailedHttpCall(call);
                applicationOnError?.Invoke(call);
            };
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
                Http.TraceData(TraceEventType.Error, 0, SerializeHttpCall(httpCall));
                Http.Flush();
            }

            /// <summary>
            /// Traces an HTTP request
            /// </summary>
            /// <param name="httpCall">The Flurl HTTP call instance, containing information about the request and response.</param>
            public static void TraceHttpCall(HttpCall httpCall)
            {
                Http.TraceData(TraceEventType.Information, 0, SerializeHttpCall(httpCall));
            }

            private static string SerializeHttpCall(HttpCall httpCall)
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                return JsonConvert.SerializeObject(httpCall, Formatting.Indented, settings);
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
        /// Custom Flurl.Http configuration settings which are specific to requests made by this SDK.
        /// </summary>
        public FlurlHttpSettings FlurlHttpSettings { get; private set; }

        /// <summary> 
        /// Additional application specific user agents which should be set in the UserAgent header on all requests.
        /// </summary>
        public List<ProductInfoHeaderValue> UserAgents { get; private set; }

        /// <summary>
		/// Clear all custom global options and set default values.
		/// </summary>
        public virtual void ResetDefaults()
        {
            FlurlHttpSettings = new FlurlHttpSettings();
            UserAgents = new List<ProductInfoHeaderValue>
            {
                new ProductInfoHeaderValue("openstack.net", GetType().GetAssemblyFileVersion())
            };
        }
    }
}
