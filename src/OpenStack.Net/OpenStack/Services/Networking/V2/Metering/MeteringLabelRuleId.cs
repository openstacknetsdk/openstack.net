namespace OpenStack.Services.Networking.V2.Metering
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a <see cref="MeteringLabelRule"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(MeteringLabelRuleId.Converter))]
    public sealed class MeteringLabelRuleId : ResourceIdentifier<MeteringLabelRuleId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabelRuleId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public MeteringLabelRuleId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="MeteringLabelRuleId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override MeteringLabelRuleId FromValue(string id)
            {
                return new MeteringLabelRuleId(id);
            }
        }
    }
}
