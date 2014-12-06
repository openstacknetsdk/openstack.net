namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;
    using IPAddressSimpleConverter = OpenStack.ObjectModel.Converters.IPAddressSimpleConverter;

#if PORTABLE
    using IPAddress = System.String;
#else
    using IPAddress = System.Net.IPAddress;
#endif

    [JsonObject(MemberSerialization.OptIn)]
    public class FloatingIp : FloatingIpData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private FloatingIpId _id;

        [JsonProperty("router_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private RouterId _routerId;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        [JsonProperty("fixed_ip_address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _fixedAddress;

        [JsonProperty("floating_ip_address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _floatingAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingIp"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected FloatingIp()
        {
        }

        public FloatingIpId Id
        {
            get
            {
                return _id;
            }
        }

        public RouterId RouterId
        {
            get
            {
                return _routerId;
            }
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        public IPAddress FixedAddress
        {
            get
            {
                return _fixedAddress;
            }
        }

        public IPAddress FloatingAddress
        {
            get
            {
                return _floatingAddress;
            }
        }
    }
}
