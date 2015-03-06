namespace OpenStack.Services.Networking.V2
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="Port"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="PortData.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(PortStatus.Converter))]
    public sealed class PortStatus : ExtensibleEnum<PortStatus>
    {
        private static readonly ConcurrentDictionary<string, PortStatus> _types =
            new ConcurrentDictionary<string, PortStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly PortStatus _active = FromName("ACTIVE");
        private static readonly PortStatus _down = FromName("DOWN");

        /// <summary>
        /// Initializes a new instance of the <see cref="PortStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private PortStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="PortStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="PortStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static PortStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new PortStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="PortStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static PortStatus Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Gets an <see cref="PortStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static PortStatus Down
        {
            get
            {
                return _down;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="PortStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override PortStatus FromName(string name)
            {
                return PortStatus.FromName(name);
            }
        }
    }
}
