namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class TokenData : ExtensibleJsonObject
    {
        [JsonProperty("catalog", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private JToken _catalog;

        [JsonProperty("expires_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _expiresAt;

        [JsonProperty("issued_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _issuedAt;

        [JsonProperty("methods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<AuthenticationMethod> _methods;

        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Domain _domain;

        [JsonProperty("project", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Project _project;

        [JsonProperty("roles", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Role> _roles;

        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private User _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected TokenData()
        {
        }

        public JToken Catalog
        {
            get
            {
                return _catalog;
            }
        }

        public DateTimeOffset? ExpiresAt
        {
            get
            {
                return _expiresAt;
            }
        }

        public DateTimeOffset? IssuedAt
        {
            get
            {
                return _issuedAt;
            }
        }

        public ImmutableArray<AuthenticationMethod> Methods
        {
            get
            {
                return _methods;
            }
        }

        public Domain Domain
        {
            get
            {
                return _domain;
            }
        }

        public Project Project
        {
            get
            {
                return _project;
            }
        }

        public ImmutableArray<Role> Roles
        {
            get
            {
                return _roles;
            }
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
