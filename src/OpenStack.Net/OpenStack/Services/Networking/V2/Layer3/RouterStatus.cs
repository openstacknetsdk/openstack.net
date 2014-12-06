namespace OpenStack.Services.Networking.V2.Layer3
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="Router"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="Router.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(RouterStatus.Converter))]
    public sealed class RouterStatus : ExtensibleEnum<RouterStatus>
    {
        private static readonly ConcurrentDictionary<string, RouterStatus> _types =
            new ConcurrentDictionary<string, RouterStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly RouterStatus _active = FromName("active");

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private RouterStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="RouterStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="RouterStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static RouterStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new RouterStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="RouterStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static RouterStatus Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="RouterStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override RouterStatus FromName(string name)
            {
                return RouterStatus.FromName(name);
            }
        }
    }
}
