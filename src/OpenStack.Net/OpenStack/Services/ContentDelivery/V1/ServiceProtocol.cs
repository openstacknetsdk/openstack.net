namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the protocol of a <see cref="ServiceDomain"/> in the Content Delivery Service.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known protocols, with added support for unknown
    /// protocols supported by a server extension.
    /// </remarks>
    /// <seealso cref="ServiceDomain.Protocol"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ServiceProtocol.Converter))]
    public sealed class ServiceProtocol : ExtensibleEnum<ServiceProtocol>
    {
        private static readonly ConcurrentDictionary<string, ServiceProtocol> _values =
            new ConcurrentDictionary<string, ServiceProtocol>(StringComparer.OrdinalIgnoreCase);
        private static readonly ServiceProtocol _http = FromName("http");
        private static readonly ServiceProtocol _https = FromName("https");

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProtocol"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private ServiceProtocol(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="ServiceProtocol"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="ServiceProtocol"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ServiceProtocol FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _values.GetOrAdd(name, i => new ServiceProtocol(i));
        }

        /// <summary>
        /// Gets a <see cref="ServiceProtocol"/> representing the HTTP protocol.
        /// </summary>
        /// <value>
        /// A <see cref="ServiceProtocol"/> representing the HTTP protocol.
        /// </value>
        public static ServiceProtocol Http
        {
            get
            {
                return _http;
            }
        }

        /// <summary>
        /// Gets a <see cref="ServiceProtocol"/> representing the HTTPS protocol.
        /// </summary>
        /// <value>
        /// A <see cref="ServiceProtocol"/> representing the HTTPS protocol.
        /// </value>
        public static ServiceProtocol Https
        {
            get
            {
                return _https;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ServiceProtocol"/> objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ServiceProtocol FromName(string name)
            {
                return ServiceProtocol.FromName(name);
            }
        }
    }
}
