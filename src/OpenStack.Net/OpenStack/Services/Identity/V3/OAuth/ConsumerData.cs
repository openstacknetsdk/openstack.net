namespace OpenStack.Services.Identity.V3.OAuth
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ConsumerData : ExtensibleJsonObject
    {
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ConsumerData()
        {
        }

        public ConsumerData(string description)
        {
            _description = description;
        }

        public ConsumerData(string description, params JProperty[] extensionData)
            : base(extensionData)
        {
            _description = description;
        }

        public ConsumerData(string description, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _description = description;
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }
    }
}
