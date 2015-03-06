namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class PortRequest : ExtensibleJsonObject
    {
        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PortData _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PortRequest()
        {
        }

        public PortRequest(PortData port)
        {
            _port = port;
        }

        public PortData Port
        {
            get
            {
                return _port;
            }
        }
    }
}
