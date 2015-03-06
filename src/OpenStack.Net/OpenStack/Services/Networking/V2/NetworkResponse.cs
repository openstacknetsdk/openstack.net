namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class NetworkResponse : ExtensibleJsonObject
    {
        [JsonProperty("network", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Network _network;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected NetworkResponse()
        {
        }

        public Network Network
        {
            get
            {
                return _network;
            }
        }
    }
}
