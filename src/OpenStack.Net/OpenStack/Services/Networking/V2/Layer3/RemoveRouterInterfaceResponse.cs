namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class RemoveRouterInterfaceResponse : ExtensibleJsonObject
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private RouterId _routerId;

        [JsonProperty("subnet_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        [JsonProperty("port_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PortId _portId;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveRouterInterfaceResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RemoveRouterInterfaceResponse()
        {
        }

        public RouterId RouterId
        {
            get
            {
                return _routerId;
            }
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

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }
    }
}
