namespace OpenStack.Services.Identity.V3.OAuth
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Consumer : ConsumerData
    {
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ConsumerId _id;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _secret;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Consumer()
        {
        }

        public ConsumerId Id
        {
            get
            {
                return _id;
            }
        }

        public string Secret
        {
            get
            {
                return _secret;
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
