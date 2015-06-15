namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RoleAssignment : ExtensibleJsonObject
    {
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        [JsonProperty("role", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Role _role;

        [JsonProperty("scope", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Scope _scope;

        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleAssignment"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RoleAssignment()
        {
        }

        public Role Role
        {
            get
            {
                return _role;
            }
        }

        public Scope Scope
        {
            get
            {
                return _scope;
            }
        }

        public User User
        {
            get
            {
                return _user;
            }
        }

        public ImmutableDictionary<string, string> Links
        {
            get
            {
                return _links;
            }
        }
    }
}
