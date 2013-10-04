using System;
using System.Collections.Concurrent;
using net.openstack.Core.Domain.Converters;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    /// <summary>
    /// Represents the type of a Load Balancer Node.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known load balancer types
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(LoadBalancerNodeType.Converter))]
    public sealed class LoadBalancerNodeType : IEquatable<LoadBalancerNodeType>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerNodeType> _algorithms =
            new ConcurrentDictionary<string, LoadBalancerNodeType>(StringComparer.OrdinalIgnoreCase);

        private static readonly LoadBalancerNodeType _primary = FromName("PRIMARY");
        private static readonly LoadBalancerNodeType _secondary = FromName("SECONDARY");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerNodeType"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private LoadBalancerNodeType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerNodeType"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerNodeType FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _algorithms.GetOrAdd(name, i => new LoadBalancerNodeType(i));
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerNodeType"/> representing the load balancer node PRIMARY type.
        /// </summary>
        public static LoadBalancerNodeType Primary
        {
            get { return _primary; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerNodeType"/> representing the load balancer node SECONDARY type.
        /// </summary>
        public static LoadBalancerNodeType Secondary
        {
            get { return _secondary; }
        }
        
        /// <summary>
        /// Gets the canonical name of this image state.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public bool Equals(LoadBalancerNodeType other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerNodeType"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<LoadBalancerNodeType>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(LoadBalancerNodeType obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override LoadBalancerNodeType ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
