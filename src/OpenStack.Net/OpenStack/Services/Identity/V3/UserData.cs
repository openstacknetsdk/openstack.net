namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class UserData : ExtensibleJsonObject
    {
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _email;

        [JsonProperty("default_project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _defaultProjectId;

        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected UserData()
        {
        }

        public UserData(string description, string email, ProjectId defaultProjectId, bool? enabled)
        {
            _description = description;
            _email = email;
            _defaultProjectId = defaultProjectId;
            _enabled = enabled;
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
        }

        public ProjectId DefaultProjectId
        {
            get
            {
                return _defaultProjectId;
            }
        }

        public bool? Enabled
        {
            get
            {
                return _enabled;
            }
        }
    }
}
