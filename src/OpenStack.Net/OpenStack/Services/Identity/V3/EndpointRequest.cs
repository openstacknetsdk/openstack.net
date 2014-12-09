namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

        public EndpointRequest(EndpointData data, params JProperty[] extensionData)
            : base(extensionData)
        {
            _endpoint = data;
        }

        public EndpointRequest(EndpointData data, IDictionary<string, JToken> extensionData)
            : base(extensionData)
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
