namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class User : UserData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private UserId _id;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("domain_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainId _domainId;

        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Domain _domain;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected User()
        {
        }

        public UserId Id
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Domain Domain
        {
            get
            {
                // this is populated for a User returned in a TokenData
                return _domain;
            }
        }

        public ImmutableDictionary<string, string> Links
        {
            get
            {
                return _links;
            }
        }

        public virtual DomainId DomainId
        {
            get
            {
                DomainId result = _domainId;
                if (result == null && Domain != null)
                    result = Domain.Id;

                return result;
            }
        }
    }
}
