namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the type of probe sent by a load balancer <see cref="HealthMonitor"/>
    /// to verify member state.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known probe types,
    /// with added support for unknown types supported by a server extension.
    /// </remarks>
    /// <seealso cref="HealthMonitorData.Type"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ProbeType.Converter))]
    public sealed class ProbeType : ExtensibleEnum<ProbeType>
    {
        private static readonly ConcurrentDictionary<string, ProbeType> _types =
            new ConcurrentDictionary<string, ProbeType>(StringComparer.OrdinalIgnoreCase);
        private static readonly ProbeType _ping = FromName("PING");
        private static readonly ProbeType _tcp = FromName("TCP");
        private static readonly ProbeType _http = FromName("HTTP");
        private static readonly ProbeType _https = FromName("HTTPS");

        /// <summary>
        /// Initializes a new instance of the <see cref="ProbeType"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private ProbeType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="ProbeType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="ProbeType"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ProbeType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new ProbeType(i));
        }

        /// <summary>
        /// Gets an <see cref="ProbeType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static ProbeType Ping
        {
            get
            {
                return _ping;
            }
        }

        /// <summary>
        /// Gets a <see cref="ProbeType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static ProbeType Tcp
        {
            get
            {
                return _tcp;
            }
        }

        /// <summary>
        /// Gets a <see cref="ProbeType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static ProbeType Http
        {
            get
            {
                return _http;
            }
        }

        /// <summary>
        /// Gets a <see cref="ProbeType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static ProbeType Https
        {
            get
            {
                return _https;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ProbeType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ProbeType FromName(string name)
            {
                return ProbeType.FromName(name);
            }
        }
    }
}
