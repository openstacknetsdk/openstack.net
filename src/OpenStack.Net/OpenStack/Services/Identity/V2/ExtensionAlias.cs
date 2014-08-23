namespace OpenStack.Services.Identity.V2
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique name of an API extension.
    /// </summary>
    /// <seealso cref="Extension.Alias"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ExtensionAlias.Converter))]
    public sealed class ExtensionAlias : ResourceIdentifier<ExtensionAlias>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionAlias"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public ExtensionAlias(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ExtensionAlias"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ExtensionAlias FromValue(string id)
            {
                return new ExtensionAlias(id);
            }
        }
    }
}
