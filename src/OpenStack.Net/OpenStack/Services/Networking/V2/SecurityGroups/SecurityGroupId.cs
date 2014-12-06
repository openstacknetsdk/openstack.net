namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a <see cref="SecurityGroup"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(SecurityGroupId.Converter))]
    public sealed class SecurityGroupId : ResourceIdentifier<SecurityGroupId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public SecurityGroupId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="SecurityGroupId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override SecurityGroupId FromValue(string id)
            {
                return new SecurityGroupId(id);
            }
        }
    }
}
