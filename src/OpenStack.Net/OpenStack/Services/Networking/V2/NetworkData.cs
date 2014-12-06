namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class NetworkData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        [JsonProperty("shared", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _shared;

        [JsonProperty("admin_state_up", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _adminStateUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected NetworkData()
        {
        }

        public NetworkData(string name)
            : this(name, null, null, null)
        {
        }

        public NetworkData(string name, ProjectId projectId, bool? shared, bool? adminStateUp)
        {
            _name = name;
            _projectId = projectId;
            _shared = shared;
            _adminStateUp = adminStateUp;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        public bool? Shared
        {
            get
            {
                return _shared;
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
