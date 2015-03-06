namespace OpenStack.Services.Networking.V2
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="Network"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="Network.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(NetworkStatus.Converter))]
    public sealed class NetworkStatus : ExtensibleEnum<NetworkStatus>
    {
        private static readonly ConcurrentDictionary<string, NetworkStatus> _types =
            new ConcurrentDictionary<string, NetworkStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly NetworkStatus _active = FromName("ACTIVE");

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private NetworkStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="NetworkStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="NetworkStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static NetworkStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new NetworkStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="NetworkStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static NetworkStatus Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="NetworkStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override NetworkStatus FromName(string name)
            {
                return NetworkStatus.FromName(name);
            }
        }
    }
}
