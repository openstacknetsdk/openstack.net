namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserRequest : ExtensibleJsonObject
    {
        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private UserData _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected UserRequest()
        {
        }

        public UserRequest(UserData user)
        {
        }

        public UserRequest(UserData user, params JProperty[] extensionData)
            : base(extensionData)
        {
        }

        public UserRequest(UserData user, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
        }

        public UserData User
        {
            get
            {
                return _user;
            }
        }
    }
}
