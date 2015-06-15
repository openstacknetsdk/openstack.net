namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
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

        public UserData User
        {
            get
            {
                return _user;
            }
        }
    }
}
