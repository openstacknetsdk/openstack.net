namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Policy : PolicyData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PolicyId _id;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Dictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Policy"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Policy()
        {
        }

        public PolicyId Id
        {
            get
            {
                return _id;
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
