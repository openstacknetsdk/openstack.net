namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class MediaType : ExtensibleJsonObject
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _type;

        [JsonProperty("base", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _base;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaType"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MediaType()
        {
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public string BaseType
        {
            get
            {
                return _base;
            }
        }
    }
}
