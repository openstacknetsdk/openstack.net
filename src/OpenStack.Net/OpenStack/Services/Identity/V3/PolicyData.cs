namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class PolicyData : ExtensibleJsonObject
    {
        [JsonProperty("blob", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private JObject _blob;

        [JsonProperty("project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _type;

        [JsonProperty("user_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private UserId _userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PolicyData()
        {
        }

        public PolicyData(JObject blob, string type, ProjectId projectId, UserId userId)
        {
            _blob = blob;
            _type = type;
            _projectId = projectId;
            _userId = userId;
        }

        public JObject Blob
        {
            get
            {
                return _blob;
            }
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public UserId UserId
        {
            get
            {
                return _userId;
            }
        }
    }
}
