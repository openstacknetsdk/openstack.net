namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Project : ProjectData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _id;

        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Domain _domain;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Dictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Project()
        {
        }

        public ProjectId Id
        {
            get
            {
                return _id;
            }
        }

        public Domain Domain
        {
            get
            {
                // this is populated for a Project returned in a TokenData
                return _domain;
            }
        }

        public ReadOnlyDictionary<string, string> Links
        {
            get
            {
                if (_links == null)
                    return null;

                return new ReadOnlyDictionary<string, string>(_links);
            }
        }

        public override DomainId DomainId
        {
            get
            {
                DomainId result = base.DomainId;
                if (result == null && Domain != null)
                    result = Domain.Id;

                return result;
            }
        }
    }
}
