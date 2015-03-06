namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;

#if PORTABLE
    using IPAddress = System.String;
#else
    using IPAddress = System.Net.IPAddress;
#endif

    [JsonObject(MemberSerialization.OptIn)]
    public class MemberData : ExtensibleJsonObject
    {
        [JsonProperty("pool_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PoolId _poolId;

        [JsonProperty("address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private IPAddress _address;

        [JsonProperty("protocol", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerProtocol _protocol;

        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _port;

        [JsonProperty("weight", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _weight;

        [JsonProperty("admin_state_up", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _adminStateUp;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MemberData()
        {
        }

        public MemberData(PoolId poolId, IPAddress address, LoadBalancerProtocol protocol, int? port)
        {
            _poolId = poolId;
            _address = address;
            _protocol = protocol;
            _port = port;
        }

        public MemberData(PoolId poolId, IPAddress address, LoadBalancerProtocol protocol, int? port, int? weight, bool? adminStateUp, ProjectId projectId)
        {
            _poolId = poolId;
            _address = address;
            _protocol = protocol;
            _port = port;
            _weight = weight;
            _adminStateUp = adminStateUp;
            _projectId = projectId;
        }

        public PoolId PoolId
        {
            get
            {
                return _poolId;
            }
        }

        public IPAddress Address
        {
            get
            {
                return _address;
            }
        }

        public LoadBalancerProtocol Protocol
        {
            get
            {
                return _protocol;
            }
        }

        public int? Port
        {
            get
            {
                return _port;
            }
        }

        public int? Weight
        {
            get
            {
                return _weight;
            }
        }

        public bool? AdminStateUp
        {
            get
            {
                return _adminStateUp;
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
