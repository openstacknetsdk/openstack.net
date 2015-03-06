namespace OpenStack.Services.Networking.V2
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using Link = Identity.Link;

    [JsonObject(MemberSerialization.OptIn)]
    public class Resource : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("collection", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _collection;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Resource()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Collection
        {
            get
            {
                return _collection;
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
