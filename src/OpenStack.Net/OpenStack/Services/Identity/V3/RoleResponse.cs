namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RoleResponse : ExtensibleJsonObject
    {
        [JsonProperty("role", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Role _role;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RoleResponse()
        {
        }

        public Role Role
        {
            get
            {
                return _role;
            }
        }
    }
}
