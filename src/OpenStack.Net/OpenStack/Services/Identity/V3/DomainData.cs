﻿namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class DomainData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected DomainData()
        {
        }

        public DomainData(string name, string description, bool? enabled)
        {
            _name = name;
            _description = description;
            _enabled = enabled;
        }

        public DomainData(string name, string description, bool? enabled, params JProperty[] extensionData)
            : base(extensionData)
        {
            _name = name;
            _description = description;
            _enabled = enabled;
        }

        public DomainData(string name, string description, bool? enabled, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _name = name;
            _description = description;
            _enabled = enabled;
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

        public bool? Enabled
        {
            get
            {
                return _enabled;
            }
        }
    }
}
