namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Domain : DomainData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainId _id;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Domain"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Domain()
        {
        }

        public DomainId Id
        {
            get
            {
                return _id;
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
