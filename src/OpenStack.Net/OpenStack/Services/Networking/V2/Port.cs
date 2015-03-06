namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Port : PortData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PortId _id;

        [JsonProperty("device_owner", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _deviceOwner;

        [JsonProperty("device_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _deviceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Port"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Port()
        {
        }

        public PortId Id
        {
            get
            {
                return _id;
            }
        }

        public string DeviceOwner
        {
            get
            {
                return _deviceOwner;
            }
        }

        public string DeviceId
        {
            get
            {
                return _deviceId;
            }
        }
    }
}
