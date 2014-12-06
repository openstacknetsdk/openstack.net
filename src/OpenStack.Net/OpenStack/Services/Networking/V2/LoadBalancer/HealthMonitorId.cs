namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a <see cref="HealthMonitor"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(HealthMonitorId.Converter))]
    public sealed class HealthMonitorId : ResourceIdentifier<HealthMonitorId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitorId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public HealthMonitorId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="HealthMonitorId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override HealthMonitorId FromValue(string id)
            {
                return new HealthMonitorId(id);
            }
        }
    }
}
