namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of a <see cref="SecurityGroupRule"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(SecurityGroupRuleId.Converter))]
    public sealed class SecurityGroupRuleId : ResourceIdentifier<SecurityGroupRuleId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupRuleId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public SecurityGroupRuleId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="SecurityGroupRuleId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override SecurityGroupRuleId FromValue(string id)
            {
                return new SecurityGroupRuleId(id);
            }
        }
    }
}
