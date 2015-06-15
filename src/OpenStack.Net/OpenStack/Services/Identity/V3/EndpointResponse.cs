namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class EndpointResponse : ExtensibleJsonObject
    {
        [JsonProperty("endpoint", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Endpoint _endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected EndpointResponse()
        {
        }

        public Endpoint Endpoint
        {
            get
            {
                return _endpoint;
            }
        }
    }
}
