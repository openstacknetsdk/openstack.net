namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class Router : RouterData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private RouterId _id;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private RouterStatus _status;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Router"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Router()
        {
        }

        public RouterId Id
        {
            get
            {
                return _id;
            }
        }

        public RouterStatus Status
        {
            get
            {
                return _status;
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
