namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a subsystem in the Content Delivery Service.
    /// </summary>
    /// <seealso cref="IContentDeliveryService.PrepareGetSubsystemHealthAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(SubsystemId.Converter))]
    public sealed class SubsystemId : ResourceIdentifier<SubsystemId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubsystemId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public SubsystemId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="SubsystemId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override SubsystemId FromValue(string id)
            {
                return new SubsystemId(id);
            }
        }
    }
}
