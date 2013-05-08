using System;
using System.Collections.Concurrent;
using net.openstack.Core.Domain.Converters;
using net.openstack.Core.Providers;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    /// <summary>
    /// Represents the condition of a Load Balancer Node.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known image states,
    /// with added support for unknown states returned by an image extension.
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof(LoadBalancerNodeCondition.Converter))]
    public sealed class LoadBalancerNodeCondition : IEquatable<LoadBalancerNodeCondition>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerNodeCondition> _algorithms =
            new ConcurrentDictionary<string, LoadBalancerNodeCondition>(StringComparer.OrdinalIgnoreCase);

        private static readonly LoadBalancerNodeCondition _enabled = FromName("ENABLED");
        private static readonly LoadBalancerNodeCondition _disabled = FromName("DISABLED");
        private static readonly LoadBalancerNodeCondition _draining = FromName("DRAINING");

        //public enum 
        //{
        //    ENABLED,
        //    DISABLED,
        //    DRAINING
        //}

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerNodeCondition"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private LoadBalancerNodeCondition(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerNodeCondition"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerNodeCondition FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _algorithms.GetOrAdd(name, i => new LoadBalancerNodeCondition(i));
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerNodeCondition"/> representing a load balancer node in the ENABLED condition.
        /// </summary>
        public static LoadBalancerNodeCondition Enabled
        {
            get { return _enabled; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerNodeCondition"/> representing a load balancer node in the DISABLED condition.
        /// </summary>
        public static LoadBalancerNodeCondition Disabled
        {
            get { return _disabled; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerNodeCondition"/> representing a load balancer node in the DRAINING condition.
        /// </summary>
        public static LoadBalancerNodeCondition Draining
        {
            get { return _draining; }
        }

        /// <summary>
        /// Gets the canonical name of this load balancer node condition.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public bool Equals(LoadBalancerNodeCondition other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerNodeCondition"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<LoadBalancerNodeCondition>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(LoadBalancerNodeCondition obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override LoadBalancerNodeCondition ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}
