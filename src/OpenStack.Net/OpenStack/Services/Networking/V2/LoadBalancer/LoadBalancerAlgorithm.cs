namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the a load balancer algorithm.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known load balancer algorithms,
    /// with added support for unknown algorithms supported by a server extension.
    /// </remarks>
    /// <seealso cref="PoolData.Algorithm"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(LoadBalancerAlgorithm.Converter))]
    public sealed class LoadBalancerAlgorithm : ExtensibleEnum<LoadBalancerAlgorithm>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerAlgorithm> _types =
            new ConcurrentDictionary<string, LoadBalancerAlgorithm>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerAlgorithm"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private LoadBalancerAlgorithm(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerAlgorithm"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="LoadBalancerAlgorithm"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerAlgorithm FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new LoadBalancerAlgorithm(i));
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerAlgorithm"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override LoadBalancerAlgorithm FromName(string name)
            {
                return LoadBalancerAlgorithm.FromName(name);
            }
        }
    }
}
