namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="Pool"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="Pool.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(PoolStatus.Converter))]
    public sealed class PoolStatus : ExtensibleEnum<PoolStatus>
    {
        private static readonly ConcurrentDictionary<string, PoolStatus> _types =
            new ConcurrentDictionary<string, PoolStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly PoolStatus _active = FromName("ACTIVE");

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private PoolStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="PoolStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="PoolStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static PoolStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new PoolStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="PoolStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static PoolStatus Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="PoolStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override PoolStatus FromName(string name)
            {
                return PoolStatus.FromName(name);
            }
        }
    }
}
