namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents a load balancer protocol.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known load balancer protocols,
    /// with added support for unknown protocols supported by a server extension.
    /// </remarks>
    /// <seealso cref="MemberData.Protocol"/>
    /// <seealso cref="VirtualAddressData.Protocol"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(LoadBalancerProtocol.Converter))]
    public sealed class LoadBalancerProtocol : ExtensibleEnum<LoadBalancerProtocol>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerProtocol> _types =
            new ConcurrentDictionary<string, LoadBalancerProtocol>(StringComparer.OrdinalIgnoreCase);
        private static readonly LoadBalancerProtocol _tcp = FromName("TCP");
        private static readonly LoadBalancerProtocol _http = FromName("HTTP");
        private static readonly LoadBalancerProtocol _https = FromName("HTTPS");

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerProtocol"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private LoadBalancerProtocol(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerProtocol"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="LoadBalancerProtocol"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerProtocol FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new LoadBalancerProtocol(i));
        }

        /// <summary>
        /// Gets a <see cref="LoadBalancerProtocol"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static LoadBalancerProtocol Tcp
        {
            get
            {
                return _tcp;
            }
        }

        /// <summary>
        /// Gets a <see cref="LoadBalancerProtocol"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static LoadBalancerProtocol Http
        {
            get
            {
                return _http;
            }
        }

        /// <summary>
        /// Gets a <see cref="LoadBalancerProtocol"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static LoadBalancerProtocol Https
        {
            get
            {
                return _https;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerProtocol"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override LoadBalancerProtocol FromName(string name)
            {
                return LoadBalancerProtocol.FromName(name);
            }
        }
    }
}
