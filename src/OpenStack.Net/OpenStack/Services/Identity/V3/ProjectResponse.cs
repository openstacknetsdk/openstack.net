namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectResponse : ExtensibleJsonObject
    {
        [JsonProperty("project", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Project _project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ProjectResponse()
        {
        }

        public Project Project
        {
            get
            {
                return _project;
            }
        }
    }
}
