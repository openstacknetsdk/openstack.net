namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject]
    public class ServiceData : ExtensibleJsonObject
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceData()
        {
        }

        public ServiceData(string type)
        {
            _type = type;
        }

        public ServiceData(string type, params JProperty[] extensionData)
            : base(extensionData)
        {
            _type = type;
        }

        public ServiceData(string type, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _type = type;
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }
    }
}
