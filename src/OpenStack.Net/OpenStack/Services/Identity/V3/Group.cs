namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class Group : ExtensibleJsonObject
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private GroupId _id;

        [JsonProperty("domain_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainId _domainId;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Dictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Group()
        {
        }

        public GroupId Id
        {
            get
            {
                return _id;
            }
        }

        public DomainId DomainId
        {
            get
            {
                return _domainId;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
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
    }
}
