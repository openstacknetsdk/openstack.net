namespace OpenStack.Services.Networking.V2.Provider
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents a type of a network provider.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known network types,
    /// with added support for unknown types supported by a server extension.
    /// </remarks>
    /// <seealso cref="MultiProvider.Segment.NetworkType"/>
    /// <seealso cref="ProviderExtensions.GetProviderNetworkType"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(NetworkType.Converter))]
    public sealed class NetworkType : ExtensibleEnum<NetworkType>
    {
        private static readonly ConcurrentDictionary<string, NetworkType> _types =
            new ConcurrentDictionary<string, NetworkType>(StringComparer.OrdinalIgnoreCase);
        private static readonly NetworkType _flat = FromName("flat");
        private static readonly NetworkType _vlan = FromName("vlan");
        private static readonly NetworkType _vxlan = FromName("vxlan");
        private static readonly NetworkType _gre = FromName("gre");

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkType"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private NetworkType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="NetworkType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="NetworkType"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static NetworkType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new NetworkType(i));
        }

        /// <summary>
        /// Gets an <see cref="NetworkType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static NetworkType Flat
        {
            get
            {
                return _flat;
            }
        }

        /// <summary>
        /// Gets an <see cref="NetworkType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static NetworkType Vlan
        {
            get
            {
                return _vlan;
            }
        }

        /// <summary>
        /// Gets an <see cref="NetworkType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static NetworkType Vxlan
        {
            get
            {
                return _vxlan;
            }
        }

        /// <summary>
        /// Gets an <see cref="NetworkType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static NetworkType Gre
        {
            get
            {
                return _gre;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="NetworkType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override NetworkType FromName(string name)
            {
                return NetworkType.FromName(name);
            }
        }
    }
}
