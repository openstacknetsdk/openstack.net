namespace OpenStack.Services.Identity.V3.OAuth
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ConsumerRequest : ExtensibleJsonObject
    {
        [JsonProperty("consumer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ConsumerData _consumer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ConsumerRequest()
        {
        }

        public ConsumerRequest(ConsumerData consumer)
        {
            _consumer = consumer;
        }

        public ConsumerRequest(ConsumerData consumer, params JProperty[] extensionData)
            : base(extensionData)
        {
            _consumer = consumer;
        }

        public ConsumerRequest(ConsumerData consumer, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _consumer = consumer;
        }

        public ConsumerData Consumer
        {
            get
            {
                return _consumer;
            }
        }
    }
}
