namespace OpenStack.Services.Networking.V2.Metering
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a <see cref="MeteringLabel"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(MeteringLabelId.Converter))]
    public sealed class MeteringLabelId : ResourceIdentifier<MeteringLabelId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabelId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public MeteringLabelId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="MeteringLabelId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override MeteringLabelId FromValue(string id)
            {
                return new MeteringLabelId(id);
            }
        }
    }
}
