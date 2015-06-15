namespace OpenStack.Services.Identity.V3.OAuth
{
    using Newtonsoft.Json;
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

        public ConsumerData Consumer
        {
            get
            {
                return _consumer;
            }
        }
    }
}
