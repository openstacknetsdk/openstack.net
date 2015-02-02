namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticateData : ExtensibleJsonObject
    {
        [JsonProperty("identity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private IdentityData _identity;

        [JsonProperty("scope", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AuthenticationScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AuthenticateData()
        {
        }

        public AuthenticateData(IdentityData identity, AuthenticationScope scope)
        {
            _identity = identity;
            _scope = scope;
        }
    }
}
