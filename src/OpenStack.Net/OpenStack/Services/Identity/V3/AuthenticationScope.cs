namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticationScope : ExtensibleJsonObject
    {
        [JsonProperty("project", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectData _project;

        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainData _domain;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationScope"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AuthenticationScope()
        {
        }

        public AuthenticationScope(ProjectData project)
        {
            _project = project;
        }

        public AuthenticationScope(DomainData domain)
        {
            _domain = domain;
        }

        public AuthenticationScope(ProjectData project, DomainData domain, params JProperty[] extensionData)
            : base(extensionData)
        {
            _project = project;
            _domain = domain;
        }

        public AuthenticationScope(ProjectData project, DomainData domain, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _project = project;
            _domain = domain;
        }

        public ProjectData Project
        {
            get
            {
                return _project;
            }
        }

        public DomainData Domain
        {
            get
            {
                return _domain;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class ProjectData : ExtensibleJsonObject
        {
            [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private ProjectId _projectId;

            [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private string _projectName;

            [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private DomainData _domain;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProjectData"/> class
            /// during JSON deserialization.
            /// </summary>
            [JsonConstructor]
            protected ProjectData()
            {
            }

            public ProjectData(ProjectId id)
            {
                _projectId = id;
            }

            public ProjectData(string projectName, DomainData domain)
            {
                _projectName = projectName;
                _domain = domain;
            }

            public ProjectData(ProjectId id, string projectName, DomainData domain, params JProperty[] extensionData)
                : base(extensionData)
            {
                _projectId = id;
                _projectName = projectName;
                _domain = domain;
            }

            public ProjectData(ProjectId id, string projectName, DomainData domain, IDictionary<string, JToken> extensionData)
                : base(extensionData)
            {
                _projectId = id;
                _projectName = projectName;
                _domain = domain;
            }

            public ProjectId ProjectId
            {
                get
                {
                    return _projectId;
                }
            }

            public string ProjectName
            {
                get
                {
                    return _projectName;
                }
            }

            public DomainData Domain
            {
                get
                {
                    return _domain;
                }
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class DomainData : ExtensibleJsonObject
        {
            [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private DomainId _domainId;

            /// <summary>
            /// Initializes a new instance of the <see cref="DomainData"/> class
            /// during JSON deserialization.
            /// </summary>
            [JsonConstructor]
            protected DomainData()
            {
            }

            public DomainData(DomainId domainId)
            {
                _domainId = domainId;
            }

            public DomainData(DomainId domainId, params JProperty[] extensionData)
                : base(extensionData)
            {
                _domainId = domainId;
            }

            public DomainData(DomainId domainId, IDictionary<string, JToken> extensionData)
                : base(extensionData)
            {
                _domainId = domainId;
            }

            public DomainId DomainId
            {
                get
                {
                    return _domainId;
                }
            }
        }
    }
}
