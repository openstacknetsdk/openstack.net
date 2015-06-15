namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Project : ProjectData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _id;

        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Domain _domain;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Project()
        {
        }

        public ProjectId Id
        {
            get
            {
                return _id;
            }
        }

        public Domain Domain
        {
            get
            {
                // this is populated for a Project returned in a TokenData
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

        public override DomainId DomainId
        {
            get
            {
                DomainId result = base.DomainId;
                if (result == null && Domain != null)
                    result = Domain.Id;

                return result;
            }
        }
    }
}
