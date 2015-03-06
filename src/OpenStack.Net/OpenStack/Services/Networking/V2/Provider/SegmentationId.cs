namespace OpenStack.Services.Networking.V2.Provider
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a network segment.
    /// </summary>
    /// <seealso cref="MultiProvider.Segment.SegmentationId"/>
    /// <seealso cref="ProviderExtensions.GetProviderSegmentationId"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(SegmentationId.Converter))]
    public sealed class SegmentationId : ResourceIdentifier<SegmentationId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentationId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public SegmentationId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="SegmentationId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override SegmentationId FromValue(string id)
            {
                return new SegmentationId(id);
            }
        }
    }
}
