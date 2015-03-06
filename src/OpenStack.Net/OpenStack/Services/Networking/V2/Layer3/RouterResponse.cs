namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RouterResponse : ExtensibleJsonObject
    {
        [JsonProperty("router", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Router _router;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RouterResponse()
        {
        }

        public Router Router
        {
            get
            {
                return _router;
            }
        }
    }
}
