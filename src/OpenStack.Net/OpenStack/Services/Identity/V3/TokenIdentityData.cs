namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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
            : base(new[] { AuthenticationMethod.Token })
        {
            _token = token;
        }

        public TokenIdentityData(TokenData token, params JProperty[] extensionData)
            : base(new[] { AuthenticationMethod.Token }, extensionData)
        {
            _token = token;
        }

        public TokenIdentityData(TokenData token, IDictionary<string, JToken> extensionData)
            : base(new[] { AuthenticationMethod.Token }, extensionData)
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

            public TokenData(TokenId tokenId, params JProperty[] extensionData)
                : base(extensionData)
            {
                _id = tokenId;
            }

            public TokenData(TokenId tokenId, IDictionary<string, JToken> extensionData)
                : base(extensionData)
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
