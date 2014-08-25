namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the name of an object in the <see cref="IObjectStorageService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ObjectName.Converter))]
    public sealed class ObjectName : ResourceIdentifier<ObjectName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectName"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The project identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public ObjectName(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ObjectName"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ObjectName FromValue(string id)
            {
                return new ObjectName(id);
            }
        }
    }
}
