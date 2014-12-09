namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

        public AuthenticateRequest(AuthenticateData authenticateData, params JProperty[] extensionData)
            : base(extensionData)
        {
            _auth = authenticateData;
        }

        public AuthenticateRequest(AuthenticateData authenticateData, IDictionary<string, JToken> extensionData)
            : base(extensionData)
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
