namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("domain_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainId _domainId;

        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ProjectData()
        {
        }

        public ProjectData(string name, string description, DomainId domainId, bool? enabled)
        {
            _name = name;
            _description = description;
            _domainId = domainId;
            _enabled = enabled;
        }

        public ProjectData(string name, string description, DomainId domainId, bool? enabled, params JProperty[] extensionData)
            : base(extensionData)
        {
            _name = name;
            _description = description;
            _domainId = domainId;
            _enabled = enabled;
        }

        public ProjectData(string name, string description, DomainId domainId, bool? enabled, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _name = name;
            _description = description;
            _domainId = domainId;
            _enabled = enabled;
        }

        public virtual string Name
        {
            get
            {
                return _name;
            }
        }

        public virtual string Description
        {
            get
            {
                return _description;
            }
        }

        public virtual DomainId DomainId
        {
            get
            {
                return _domainId;
            }
        }

        public virtual bool? Enabled
        {
            get
            {
                return _enabled;
            }
        }
    }
}
