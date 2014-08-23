namespace OpenStack.Services.Identity
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the unique identifier of an authentication token in the Identity Service V2 or V3.
    /// </summary>
    /// <seealso cref="V2.Token.Id"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(TokenId.Converter))]
    public sealed class TokenId : ResourceIdentifier<TokenId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenId"/> class
        /// with the specified identifier value.
        /// </summary>
        /// <param name="id">The token identifier value.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="id"/> is empty.</exception>
        public TokenId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="TokenId"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override TokenId FromValue(string id)
            {
                return new TokenId(id);
            }
        }
    }
}
