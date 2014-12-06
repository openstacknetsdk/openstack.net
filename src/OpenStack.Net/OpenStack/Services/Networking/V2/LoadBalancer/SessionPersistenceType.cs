namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the session persistence type of a <see cref="VirtualAddress"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known session persistence types,
    /// with added support for unknown types supported by a server extension.
    /// </remarks>
    /// <seealso cref="VirtualAddressData.SessionPersistence"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(SessionPersistenceType.Converter))]
    public sealed class SessionPersistenceType : ExtensibleEnum<SessionPersistenceType>
    {
        private static readonly ConcurrentDictionary<string, SessionPersistenceType> _types =
            new ConcurrentDictionary<string, SessionPersistenceType>(StringComparer.OrdinalIgnoreCase);
        private static readonly SessionPersistenceType _appCookie = FromName("APP_COOKIE");
        private static readonly SessionPersistenceType _httpCookie = FromName("HTTP_COOKIE");
        private static readonly SessionPersistenceType _sourceAddress = FromName("SOURCE_IP");

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionPersistenceType"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private SessionPersistenceType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="SessionPersistenceType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="SessionPersistenceType"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static SessionPersistenceType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new SessionPersistenceType(i));
        }

        /// <summary>
        /// Gets a <see cref="SessionPersistenceType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static SessionPersistenceType AppCookie
        {
            get
            {
                return _appCookie;
            }
        }

        /// <summary>
        /// Gets a <see cref="SessionPersistenceType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static SessionPersistenceType HttpCookie
        {
            get
            {
                return _httpCookie;
            }
        }

        /// <summary>
        /// Gets a <see cref="SessionPersistenceType"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static SessionPersistenceType SourceAddress
        {
            get
            {
                return _sourceAddress;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="SessionPersistenceType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override SessionPersistenceType FromName(string name)
            {
                return SessionPersistenceType.FromName(name);
            }
        }
    }
}
