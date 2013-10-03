using System;
using System.Collections.Concurrent;
using net.openstack.Core.Domain.Converters;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    /// <summary>
    /// Represents the state of a Virtual IP type.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known virtual IP type
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(VirtualIPType.Converter))]
    public sealed class VirtualIPType : IEquatable<VirtualIPType>
    {
        private static readonly ConcurrentDictionary<string, VirtualIPType> _algorithms =
            new ConcurrentDictionary<string, VirtualIPType>(StringComparer.OrdinalIgnoreCase);

        private static readonly VirtualIPType _public = FromName("PUBLIC");
        private static readonly VirtualIPType _serviceNet = FromName("SERVICENET");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualIPType"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private VirtualIPType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="VirtualIPType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static VirtualIPType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _algorithms.GetOrAdd(name, i => new VirtualIPType(i));
        }

        /// <summary>
        /// Gets an <see cref="VirtualIPType"/> representing a PUBLIC virtual IP.
        /// </summary>
        public static VirtualIPType Public
        {
            get { return _public; }
        }

        /// <summary>
        /// Gets an <see cref="VirtualIPType"/> representing a SERVICENET virtual IP.
        /// </summary>
        public static VirtualIPType ServiceNet
        {
            get { return _serviceNet; }
        }

        /// <summary>
        /// Gets the canonical name of this virtual IP type.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public bool Equals(VirtualIPType other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="VirtualIPType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<VirtualIPType>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(VirtualIPType obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override VirtualIPType ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
