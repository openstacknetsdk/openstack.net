namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of a <see cref="HealthMonitor"/>.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="HealthMonitor.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(HealthMonitorStatus.Converter))]
    public sealed class HealthMonitorStatus : ExtensibleEnum<HealthMonitorStatus>
    {
        private static readonly ConcurrentDictionary<string, HealthMonitorStatus> _types =
            new ConcurrentDictionary<string, HealthMonitorStatus>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitorStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private HealthMonitorStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="HealthMonitorStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="HealthMonitorStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static HealthMonitorStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new HealthMonitorStatus(i));
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="HealthMonitorStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override HealthMonitorStatus FromName(string name)
            {
                return HealthMonitorStatus.FromName(name);
            }
        }
    }
}
