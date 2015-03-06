namespace OpenStack.Services.Networking.V2.Layer3
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a <see cref="FloatingIp"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(FloatingIpId.Converter))]
    public sealed class FloatingIpId : ResourceIdentifier<FloatingIpId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingIpId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public FloatingIpId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="FloatingIpId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override FloatingIpId FromValue(string id)
            {
                return new FloatingIpId(id);
            }
        }
    }
}
