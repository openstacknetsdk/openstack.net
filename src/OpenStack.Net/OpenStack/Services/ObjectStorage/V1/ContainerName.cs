namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the name of a container in the <see cref="IObjectStorageService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ContainerName.Converter))]
    public sealed class ContainerName : ResourceIdentifier<ContainerName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerName"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The project identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public ContainerName(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ContainerName"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ContainerName FromValue(string id)
            {
                return new ContainerName(id);
            }
        }
    }
}
