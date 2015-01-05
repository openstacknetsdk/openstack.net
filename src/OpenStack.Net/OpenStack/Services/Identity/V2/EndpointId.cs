namespace OpenStack.Services.Identity.V2
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of an endpoint.
    /// </summary>
    /// <seealso cref="Endpoint.Id"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(EndpointId.Converter))]
    public sealed class EndpointId : ResourceIdentifier<EndpointId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public EndpointId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="EndpointId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override EndpointId FromValue(string id)
            {
                return new EndpointId(id);
            }
        }
    }
}
