namespace OpenStack.Services.Identity.V3.OAuth
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AccessTokenResponse : ExtensibleJsonObject
    {
        [JsonProperty("access_token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AccessToken _accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AccessTokenResponse()
        {
        }

        public AccessToken AccessToken
        {
            get
            {
                return _accessToken;
            }
        }
    }
}
