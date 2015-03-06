namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RouterRequest : ExtensibleJsonObject
    {
        [JsonProperty("router", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private RouterData _router;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RouterRequest()
        {
        }

        public RouterRequest(RouterData router)
        {
            _router = router;
        }

        public RouterData Router
        {
            get
            {
                return _router;
            }
        }
    }
}
