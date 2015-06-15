namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectRequest : ExtensibleJsonObject
    {
        private ProjectData _project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ProjectRequest()
        {
        }

        public ProjectRequest(ProjectData project)
        {
            _project = project;
        }

        public ProjectData Project
        {
            get
            {
                return _project;
            }
        }
    }
}
