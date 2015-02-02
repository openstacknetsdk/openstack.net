namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class EndpointRequest : ExtensibleJsonObject
    {
        [JsonProperty("endpoint", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private EndpointData _endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected EndpointRequest()
        {
        }

        public EndpointRequest(EndpointData data)
        {
            _endpoint = data;
        }

        public EndpointData Endpoint
        {
            get
            {
                return _endpoint;
            }
        }
    }
}
