namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class PoolData : ExtensibleJsonObject
    {
        [JsonProperty("protocol", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerProtocol _protocol;

        [JsonProperty("lb_method", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerAlgorithm _algorithm;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("subnet_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        [JsonProperty("health_monitors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<HealthMonitorId> _healthMonitors;

        [JsonProperty("admin_state_up", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _adminStateUp;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PoolData()
        {
        }

        public PoolData(LoadBalancerProtocol protocol, LoadBalancerAlgorithm algorithm, string name)
        {
            _protocol = protocol;
            _algorithm = algorithm;
            _name = name;
        }

        public PoolData(LoadBalancerProtocol protocol, LoadBalancerAlgorithm algorithm, string name, string description, SubnetId subnetId, ImmutableArray<HealthMonitorId> healthMonitors, bool? adminStateUp, ProjectId projectId)
        {
            _protocol = protocol;
            _algorithm = algorithm;
            _name = name;
            _description = description;
            _subnetId = subnetId;
            _healthMonitors = healthMonitors;
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

        public LoadBalancerAlgorithm Algorithm
        {
            get
            {
                return _algorithm;
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

        public ImmutableArray<HealthMonitorId> HealthMonitors
        {
            get
            {
                return _healthMonitors;
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
