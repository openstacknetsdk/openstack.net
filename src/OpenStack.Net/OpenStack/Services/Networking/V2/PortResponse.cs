namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class PortResponse : ExtensibleJsonObject
    {
        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Port _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PortResponse()
        {
        }

        public Port Port
        {
            get
            {
                return _port;
            }
        }
    }
}
