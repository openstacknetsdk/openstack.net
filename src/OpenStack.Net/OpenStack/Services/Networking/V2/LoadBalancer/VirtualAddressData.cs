namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;
    using IPAddressSimpleConverter = OpenStack.ObjectModel.Converters.IPAddressSimpleConverter;

#if PORTABLE
    using IPAddress = System.String;
#else
    using IPAddress = System.Net.IPAddress;
#endif

    [JsonObject(MemberSerialization.OptIn)]
    public class VirtualAddressData : ExtensibleJsonObject
    {
        [JsonProperty("protocol", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerProtocol _protocol;

        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _port;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("subnet_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        [JsonProperty("address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _address;

        [JsonProperty("pool_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PoolId _poolId;

        [JsonProperty("session_persistence", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SessionPersistence _sessionPersistence;

        [JsonProperty("connection_limit", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _connectionLimit;

        [JsonProperty("admin_state_up", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _adminStateUp;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualAddressData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected VirtualAddressData()
        {
        }

        public VirtualAddressData(LoadBalancerProtocol protocol, int? port, string name)
        {
            _protocol = protocol;
            _port = port;
            _name = name;
        }

        public VirtualAddressData(LoadBalancerProtocol protocol, int? port, string name, string description, SubnetId subnetId, IPAddress address, PoolId poolId, SessionPersistence sessionPersistence, int? connectionLimit, bool? adminStateUp, ProjectId projectId)
        {
            _protocol = protocol;
            _port = port;
            _name = name;
            _description = description;
            _subnetId = subnetId;
            _address = address;
            _poolId = poolId;
            _sessionPersistence = sessionPersistence;
            _connectionLimit = connectionLimit;
            _adminStateUp = adminStateUp;
            _projectId = projectId;
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

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public SubnetId SubnetId
        {
            get
            {
                return _subnetId;
            }
        }

        public IPAddress Address
        {
            get
            {
                return _address;
            }
        }

        public PoolId PoolId
        {
            get
            {
                return _poolId;
            }
        }

        public SessionPersistence SessionPersistence
        {
            get
            {
                return _sessionPersistence;
            }
        }

        public int? ConnectionLimit
        {
            get
            {
                return _connectionLimit;
            }
        }

        public bool? AdminStateUp
        {
            get
            {
                return _adminStateUp;
            }
        }
    }
}
