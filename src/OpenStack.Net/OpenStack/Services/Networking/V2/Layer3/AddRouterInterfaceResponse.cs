namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AddRouterInterfaceResponse : ExtensibleJsonObject
    {
        [JsonProperty("subnet_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        [JsonProperty("port_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PortId _portId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddRouterInterfaceResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AddRouterInterfaceResponse()
        {
        }

        public SubnetId SubnetId
        {
            get
            {
                return _subnetId;
            }
        }

        public PortId PortId
        {
            get
            {
                return _portId;
            }
        }
    }
}
