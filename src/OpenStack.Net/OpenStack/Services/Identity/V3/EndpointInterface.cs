namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the interface type on an <see cref="Endpoint"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known interface types,
    /// with added support for unknown types supported by a server extension.
    /// </remarks>
    /// <seealso cref="DatabaseInstance.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(EndpointInterface.Converter))]
    public sealed class EndpointInterface : ExtensibleEnum<EndpointInterface>
    {
        private static readonly ConcurrentDictionary<string, EndpointInterface> _types =
            new ConcurrentDictionary<string, EndpointInterface>(StringComparer.OrdinalIgnoreCase);
        private static readonly EndpointInterface _admin = FromName("admin");
        private static readonly EndpointInterface _public = FromName("public");
        private static readonly EndpointInterface _internal = FromName("internal");

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInterface"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private EndpointInterface(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="EndpointInterface"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="EndpointInterface"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static EndpointInterface FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new EndpointInterface(i));
        }

        /// <summary>
        /// Gets an <see cref="EndpointInterface"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static EndpointInterface Admin
        {
            get
            {
                return _admin;
            }
        }

        /// <summary>
        /// Gets an <see cref="EndpointInterface"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static EndpointInterface Public
        {
            get
            {
                return _public;
            }
        }

        /// <summary>
        /// Gets an <see cref="EndpointInterface"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static EndpointInterface Internal
        {
            get
            {
                return _internal;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="EndpointInterface"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override EndpointInterface FromName(string name)
            {
                return EndpointInterface.FromName(name);
            }
        }
    }
}
