namespace OpenStack.Services.Networking
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using Link = Identity.Link;

    [JsonObject(MemberSerialization.OptIn)]
    public class ApiVersion : ExtensibleJsonObject
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiVersionId _id;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiStatus _status;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersion"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ApiVersion()
        {
        }

        public ApiVersionId Id
        {
            get
            {
                return _id;
            }
        }

        public ApiStatus Status
        {
            get
            {
                return _status;
            }
        }

        public ImmutableArray<Link> Links
        {
            get
            {
                return _links;
            }
        }
    }
}
