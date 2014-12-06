namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualAddressResponse : ExtensibleJsonObject
    {
        [JsonProperty("vip", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private VirtualAddress _virtualAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualAddressResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected VirtualAddressResponse()
        {
        }

        public VirtualAddress VirtualAddress
        {
            get
            {
                return _virtualAddress;
            }
        }
    }
}
