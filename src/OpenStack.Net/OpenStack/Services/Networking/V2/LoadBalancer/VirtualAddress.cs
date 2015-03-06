namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualAddress : VirtualAddressData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private VirtualAddressId _id;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private VirtualAddressStatus _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualAddress"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected VirtualAddress()
        {
        }

        public VirtualAddressId Id
        {
            get
            {
                return _id;
            }
        }

        public VirtualAddressStatus Status
        {
            get
            {
                return _status;
            }
        }
    }
}
