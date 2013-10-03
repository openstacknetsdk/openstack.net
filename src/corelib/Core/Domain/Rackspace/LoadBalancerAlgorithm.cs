using System;
using System.Collections.Concurrent;
using net.openstack.Core.Domain.Converters;
using net.openstack.Core.Providers;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    /// <summary>
    /// Represents a Load Balancer algorithm.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known load balancer algorithms
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof (LoadBalancerAlgorithm.Converter))]
    public sealed class LoadBalancerAlgorithm : IEquatable<LoadBalancerAlgorithm>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerAlgorithm> _algorithms =
            new ConcurrentDictionary<string, LoadBalancerAlgorithm>(StringComparer.OrdinalIgnoreCase);

        private static readonly LoadBalancerAlgorithm _LeastConnections = FromName("LEAST_CONNECTIONS");
        private static readonly LoadBalancerAlgorithm _random = FromName("RANDOM");
        private static readonly LoadBalancerAlgorithm _roundRobin = FromName("ROUND_ROBIN");
        private static readonly LoadBalancerAlgorithm _weightedLeastConnections = FromName("WEIGHTED_LEAST_CONNECTIONS");
        private static readonly LoadBalancerAlgorithm _weightedRoundRobin = FromName("WEIGHTED_ROUND_ROBIN");


        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerAlgorithm"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private LoadBalancerAlgorithm(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerAlgorithm"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerAlgorithm FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _algorithms.GetOrAdd(name, i => new LoadBalancerAlgorithm(i));
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerAlgorithm"/> representing the LEAST_CONNECTIONS algorithms
        /// </summary>
        public static LoadBalancerAlgorithm LeastConnections
        {
            get { return _LeastConnections; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerAlgorithm"/> representing the RANDOM algorithms
        /// </summary>
        public static LoadBalancerAlgorithm Random
        {
            get { return _random; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerAlgorithm"/>  representing the ROUND_ROBIN algorithms
        /// </summary>
        public static LoadBalancerAlgorithm RoundRobin
        {
            get { return _roundRobin; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerAlgorithm"/>  representing the WEIGHTED_LEAST_CONNECTIONS algorithms
        /// </summary>
        public static LoadBalancerAlgorithm WeightedLeastConnections
        {
            get { return _weightedLeastConnections; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerAlgorithm"/> representing the WEIGHTED_ROUND_ROBIN algorithms
        /// </summary>
        public static LoadBalancerAlgorithm WeightedRoundRobin
        {
            get { return _weightedRoundRobin; }
        }

        /// <summary>
        /// Gets the canonical name of this image state.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <inheritdoc/>
        public bool Equals(LoadBalancerAlgorithm other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerAlgorithm"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<LoadBalancerAlgorithm>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(LoadBalancerAlgorithm obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override LoadBalancerAlgorithm ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}