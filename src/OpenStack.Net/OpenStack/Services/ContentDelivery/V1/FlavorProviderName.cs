namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the name of a <see cref="FlavorProvider"/> associated with a <see cref="Flavor"/> resource in the
    /// Content Delivery Service.
    /// </summary>
    /// <seealso cref="FlavorProvider"/>
    /// <seealso cref="IContentDeliveryService"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(FlavorProviderName.Converter))]
    public sealed class FlavorProviderName : ResourceIdentifier<FlavorProviderName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlavorProviderName"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public FlavorProviderName(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="FlavorProviderName"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override FlavorProviderName FromValue(string id)
            {
                return new FlavorProviderName(id);
            }
        }
    }
}
