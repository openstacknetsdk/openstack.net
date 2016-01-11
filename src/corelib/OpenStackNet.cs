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
        /// <para>Modify using <see cref="Configure(Action{OpenStackNetConfigurationOptions})"/>.</para>
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

        private static OpenStackNetConfigurationOptions _config;
        private static readonly object ConfigureLock = new object();
        private static bool _isConfigured;

        /// <summary>
        /// <para>DEPRECATED. This no longer needs to be explicityly called, unless you require customizations. In that case, use <see cref="Configure(Action{OpenStackNetConfigurationOptions})"/> .</para>
        /// Initializes the SDK using the default configuration.
        /// </summary>
        [Obsolete("This will be removed in v2.0. Use Configure(Action{OpenStackNetConfigurationOptions}) instead if you need to customize anything.")]
        public static void Configure()
        {
            Configure(null);
        }

        /// <summary>
        /// <para>Provides thread-safe accesss to OpenStack.NET's global configuration options.</para>
        /// <para>Can only be called once at application start-up, before instantiating any OpenStack.NET objects.</para>
        /// </summary>
        /// <param name="configure">Additional configuration of OpenStack.NET's global settings.</param>
        public static void Configure(Action<OpenStackNetConfigurationOptions> configure)
        {
            Configure(null, configure);
        }

        /// <summary>
        /// <para>DEPRECATED, use <see cref="Configure(Action{OpenStackNetConfigurationOptions})"/> instead.</para>
        /// <para>Provides thread-safe accesss to OpenStack.NET's global configuration options.</para>
        /// <para>Can only be called once at application start-up, before instantiating any OpenStack.NET objects.</para>
        /// </summary>
        /// <param name="configureFlurl">Addtional configuration of the OpenStack.NET Flurl client settings <seealso cref="Flurl.Http.FlurlHttp.Configure" />.</param>
        /// <param name="configure">Additional configuration of OpenStack.NET's global settings.</param>
        [Obsolete("This will be removed in v2.0. Use Configure(Action{OpenStackNetConfigurationOptions}) instead.")]
        public static void Configure(Action<FlurlHttpSettings> configureFlurl, Action<OpenStackNetConfigurationOptions> configure)
        {
            lock (ConfigureLock)
            {
                if (_isConfigured)
                {
                    // Check if a user is attempting to apply custom configuration after the default config has been applied
                    if(configureFlurl != null || configure != null)
                        Trace.TraceError("Ignoring additional call to OpenStackNet.Configure. It can only be called once at application start-up, before instantiating any OpenStack.NET objects.");

                    return;
                }

                // TODO: Use the line below once we hit 2.0 and configureFlurl is deprecated
                // _config = new OpenStackNetConfigurationOptions(configure);
                _config = new OpenStackNetConfigurationOptions(options =>
                {
                    configureFlurl?.Invoke(options.FlurlHttpSettings);
                    configure?.Invoke(options);
                });
                _isConfigured = true;
            }
        }

        /// <summary>
        /// Resets all configuration (OpenStack.NET, Flurl and Json.NET) so that <see cref="Configure(Action{OpenStackNetConfigurationOptions})"/> can be called again.
        /// </summary>
        public static void ResetDefaults()
        {
            lock (ConfigureLock)
            {
                _config = null;
                _isConfigured = false;
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
    /// A readonly set of properties that affect OpenStack.NET's behavior.
    /// <para>To configure, pass a custom action via the static <see cref="OpenStackNet.Configure(Action{OpenStackNetConfigurationOptions})"/> method.</para>
    /// </summary>
    public class OpenStackNetConfigurationOptions
    {
        private readonly bool _isInitialized;
        private readonly FlurlHttpSettings _flurlHttpSettings;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly List<ProductInfoHeaderValue> _userAgents;

        /// <summary/>
        protected internal OpenStackNetConfigurationOptions(Action<OpenStackNetConfigurationOptions> configure = null)
        {
            _flurlHttpSettings = new FlurlHttpSettings();
            _jsonSerializerSettings = new JsonSerializerSettings();
            _userAgents = new List<ProductInfoHeaderValue>
            {
                new ProductInfoHeaderValue("openstack.net", GetType().GetAssemblyFileVersion())
            };

            configure?.Invoke(this);
            ApplyDefaults();
            _isInitialized = true;
        }

        /// <summary>
        /// Custom Flurl.Http configuration settings which are specific to requests made by this SDK.
        /// </summary>
        public FlurlHttpSettings FlurlHttpSettings
        {
            get
            {
                if(_isInitialized)
                    return _flurlHttpSettings.Clone();

                return _flurlHttpSettings;
            }
        }

        /// <summary>
        /// Custom Json.NET configuration settings which are specific to requests made by this SDK.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (_isInitialized)
                    return _jsonSerializerSettings.Clone();

                return _jsonSerializerSettings;
            }
        }

        /// <summary> 
        /// Additional application specific user agents which should be set in the UserAgent header on all requests made by this SDK.
        /// </summary>
        public IList<ProductInfoHeaderValue> UserAgents
        {
            get
            {
                if(_isInitialized)
                    return _userAgents.AsReadOnly();

                return _userAgents;
            }
        }

        private void ApplyDefaults()
        {
            //
            // Apply our default settings on top of user customizations, hopefully without clobbering anything
            //
            _jsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            _jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            _jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            if(!(_jsonSerializerSettings.ContractResolver is OpenStackContractResolver))
                _jsonSerializerSettings.ContractResolver = new OpenStackContractResolver();
            
            _flurlHttpSettings.JsonSerializer = new NewtonsoftJsonSerializer(_jsonSerializerSettings);

            // When in test mode (set via HttpTest), this will be a custom class (TestHttpClientFactory)
            if (_flurlHttpSettings.HttpClientFactory?.GetType() == typeof(DefaultHttpClientFactory))
                _flurlHttpSettings.HttpClientFactory = new AuthenticatedHttpClientFactory();

            // Apply our event handling and optionally include any custom application handlers
            var applicationBeforeCall = _flurlHttpSettings.BeforeCall;
            _flurlHttpSettings.BeforeCall = call =>
            {
                SetUserAgentHeader(call);
                applicationBeforeCall?.Invoke(call);
            };

            var applicationAfterCall = _flurlHttpSettings.AfterCall;
            _flurlHttpSettings.AfterCall = call =>
            {
                OpenStackNet.Tracing.TraceHttpCall(call);
                applicationAfterCall?.Invoke(call);
            };

            var applicationOnError = _flurlHttpSettings.OnError;
            _flurlHttpSettings.OnError = call =>
            {
                OpenStackNet.Tracing.TraceFailedHttpCall(call);
                applicationOnError?.Invoke(call);
            };
        }

        private void SetUserAgentHeader(HttpCall call)
        {
            foreach (ProductInfoHeaderValue userAgent in UserAgents)
            {
                call.Request.Headers.UserAgent.Add(userAgent);
            }
        }
    }
}
