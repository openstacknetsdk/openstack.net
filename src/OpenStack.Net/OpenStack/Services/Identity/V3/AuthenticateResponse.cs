namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticateResponse : ExtensibleJsonObject
    {
        [JsonProperty("token", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private TokenData _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AuthenticateResponse()
        {
        }

        public TokenData Token
        {
            get
            {
                return _token;
            }
        }
    }
}
