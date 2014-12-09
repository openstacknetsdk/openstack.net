namespace OpenStack.Services.Identity.V3.OAuth
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ConsumerResponse : ExtensibleJsonObject
    {
        private Consumer _consumer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ConsumerResponse()
        {
        }

        public Consumer Consumer
        {
            get
            {
                return _consumer;
            }
        }
    }
}
