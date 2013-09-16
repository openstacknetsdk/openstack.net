namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core.Domain.Converters;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the type of a reboot operation.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known reboot types,
    /// with added support for unknown types returned by a server extension.
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(RebootType.Converter))]
    public sealed class RebootType : IEquatable<RebootType>
    {
        private static readonly ConcurrentDictionary<string, RebootType> _types =
            new ConcurrentDictionary<string, RebootType>(StringComparer.OrdinalIgnoreCase);
        private static readonly RebootType _hard = FromName("HARD");
        private static readonly RebootType _soft = FromName("SOFT");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="RebootType"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private RebootType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="RebootType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static RebootType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new RebootType(i));
        }

        /// <summary>
        /// Gets a <see cref="RebootType"/> representing the equivalent of cycling power to the server.
        /// </summary>
        public static RebootType Hard
        {
            get
            {
                return _hard;
            }
        }

        /// <summary>
        /// Gets a <see cref="RebootType"/> representing a reboot performed by signaling the server's
        /// operating system to restart, allowing for graceful shutdown of currently executing processes.
        /// </summary>
        public static RebootType Soft
        {
            get
            {
                return _soft;
            }
        }

        /// <summary>
        /// Gets the canonical name of this reboot type.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(RebootType other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="RebootType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<RebootType>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(RebootType obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override RebootType ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
