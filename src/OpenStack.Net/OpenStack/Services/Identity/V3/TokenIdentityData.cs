namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class TokenIdentityData : IdentityData
    {
        [JsonProperty("token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private TokenData _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIdentityData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected TokenIdentityData()
        {
        }

        public TokenIdentityData(TokenData token)
            : base(ImmutableArray.Create(AuthenticationMethod.Token))
        {
            _token = token;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class TokenData : ExtensibleJsonObject
        {
            [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private TokenId _id;

            /// <summary>
            /// Initializes a new instance of the <see cref="TokenData"/> class
            /// during JSON deserialization.
            /// </summary>
            [JsonConstructor]
            protected TokenData()
            {
            }

            public TokenData(TokenId tokenId)
            {
                _id = tokenId;
            }

            public TokenId Id
            {
                get
                {
                    return _id;
                }
            }
        }
    }
}
