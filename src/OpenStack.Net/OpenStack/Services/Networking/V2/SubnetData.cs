namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;
    using System.Collections.Immutable;
    using IPAddressSimpleConverter = OpenStack.ObjectModel.Converters.IPAddressSimpleConverter;

#if PORTABLE
    using IPAddress = System.String;
#else
    using IPAddress = System.Net.IPAddress;
#endif

    [JsonObject(MemberSerialization.OptIn)]
    public class SubnetData : ExtensibleJsonObject
    {
        [JsonProperty("network_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private NetworkId _networkId;

        [JsonProperty("cidr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _cidr;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("ip_version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _ipVersion;

        [JsonProperty("allocation_pools", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<AllocationPool> _allocationPools;

        [JsonProperty("gateway_ip", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _gatewayAddress;

        [JsonProperty("enable_dhcp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _enableDhcp;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubnetData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SubnetData()
        {
        }

        public SubnetData(NetworkId networkId, string cidr, string name)
        {
            _networkId = networkId;
            _cidr = cidr;
            _name = name;
        }

        public SubnetData(NetworkId networkId, string cidr, string name, int? ipVersion, ImmutableArray<AllocationPool> allocationPools, IPAddress gatewayAddress, bool? enableDhcp, ProjectId projectId)
        {
            _networkId = networkId;
            _cidr = cidr;
            _name = name;
            _ipVersion = ipVersion;
            _allocationPools = allocationPools;
            _gatewayAddress = gatewayAddress;
            _enableDhcp = enableDhcp;
            _projectId = projectId;
        }

        public NetworkId NetworkId
        {
            get
            {
                return _networkId;
            }
        }

        public string Cidr
        {
            get
            {
                return _cidr;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int? IpVersion
        {
            get
            {
                return _ipVersion;
            }
        }

        public ImmutableArray<AllocationPool> AllocationPools
        {
            get
            {
                return _allocationPools;
            }
        }

        public IPAddress GatewayAddress
        {
            get
            {
                return _gatewayAddress;
            }
        }

        public bool? EnableDhcp
        {
            get
            {
                return _enableDhcp;
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
