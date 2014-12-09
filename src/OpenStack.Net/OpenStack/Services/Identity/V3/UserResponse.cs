namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserResponse : ExtensibleJsonObject
    {
        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected UserResponse()
        {
        }

        public User User
        {
            get
            {
                return _user;
            }
        }
    }
}
