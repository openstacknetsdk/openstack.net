﻿namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Service : ServiceData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ServiceId _id;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Service()
        {
        }

        public ServiceId Id
        {
            get
            {
                return _id;
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

        public ImmutableDictionary<string, string> Links
        {
            get
            {
                return _links;
            }
        }
    }
}
