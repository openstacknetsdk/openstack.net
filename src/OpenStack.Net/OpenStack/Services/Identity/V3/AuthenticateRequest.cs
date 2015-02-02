namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticateRequest : ExtensibleJsonObject
    {
        [JsonProperty("auth", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AuthenticateData _auth;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AuthenticateRequest()
        {
        }

        public AuthenticateRequest(AuthenticateData authenticateData)
        {
            _auth = authenticateData;
        }

        public AuthenticateData AuthenticateData
        {
            get
            {
                return _auth;
            }
        }
    }
}
