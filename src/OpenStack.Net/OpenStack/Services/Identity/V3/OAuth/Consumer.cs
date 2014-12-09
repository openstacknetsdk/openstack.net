namespace OpenStack.Services.Identity.V3.OAuth
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Consumer : ConsumerData
    {
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ConsumerId _id;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _secret;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Dictionary<string, string> _links;

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

        public ReadOnlyDictionary<string, string> Links
        {
            get
            {
                if (_links == null)
                    return null;

                return new ReadOnlyDictionary<string, string>(_links);
            }
        }
    }
}
