namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

        public ProjectRequest(ProjectData project, params JProperty[] extensionData)
            : base(extensionData)
        {
            _project = project;
        }

        public ProjectRequest(ProjectData project, IDictionary<string, JToken> extensionData)
            : base(extensionData)
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
