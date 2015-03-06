namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="VirtualAddress"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="VirtualAddress.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(VirtualAddressStatus.Converter))]
    public sealed class VirtualAddressStatus : ExtensibleEnum<VirtualAddressStatus>
    {
        private static readonly ConcurrentDictionary<string, VirtualAddressStatus> _types =
            new ConcurrentDictionary<string, VirtualAddressStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly VirtualAddressStatus _active = FromName("ACTIVE");

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualAddressStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private VirtualAddressStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="VirtualAddressStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="VirtualAddressStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static VirtualAddressStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new VirtualAddressStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="VirtualAddressStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static VirtualAddressStatus Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="VirtualAddressStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override VirtualAddressStatus FromName(string name)
            {
                return VirtualAddressStatus.FromName(name);
            }
        }
    }
}
