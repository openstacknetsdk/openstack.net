﻿namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class GroupData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("domain_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainId _domainId;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected GroupData()
        {
        }

        public GroupData(string name)
            : this(name, null, null)
        {
        }

        public GroupData(string name, DomainId domainId, string description)
        {
            _name = name;
            _domainId = domainId;
            _description = description;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public DomainId DomainId
        {
            get
            {
                return _domainId;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }
    }
}
