namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ApiVersionResponse : ExtensibleJsonObject
    {
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiVersion _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ApiVersionResponse()
        {
        }

        public ApiVersion Version
        {
            get
            {
                return _version;
            }
        }
    }
}
