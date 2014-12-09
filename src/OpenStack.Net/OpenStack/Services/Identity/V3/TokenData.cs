namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.ObjectModel;
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
        private AuthenticationMethod[] _methods;

        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Domain _domain;

        [JsonProperty("project", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Project _project;

        [JsonProperty("roles", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Role[] _roles;

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

        public ReadOnlyCollection<AuthenticationMethod> Methods
        {
            get
            {
                if (_methods == null)
                    return null;

                return new ReadOnlyCollection<AuthenticationMethod>(_methods);
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

        public ReadOnlyCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                    return null;

                return new ReadOnlyCollection<Role>(_roles);
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
