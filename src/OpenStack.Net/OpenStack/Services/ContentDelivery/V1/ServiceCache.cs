namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceCache : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="TimeToLive"/> property.
        /// </summary>
        [JsonProperty("ttl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private long? _ttl;

        /// <summary>
        /// This is the backing field for the <see cref="Rules"/> property.
        /// </summary>
        [JsonProperty("rules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceCacheRule> _rules;

        public ServiceCache(string name, TimeSpan? timeToLive, ImmutableArray<ServiceCacheRule> rules)
        {
            _name = name;
            if (timeToLive.HasValue)
                _ttl = (long)timeToLive.Value.TotalSeconds;
            _rules = rules;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCache"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceCache()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public TimeSpan? TimeToLive
        {
            get
            {
                if (_ttl == null)
                    return null;

                return TimeSpan.FromSeconds(_ttl.Value);
            }
        }

        public ImmutableArray<ServiceCacheRule> Rules
        {
            get
            {
                return _rules;
            }
        }
    }
}
