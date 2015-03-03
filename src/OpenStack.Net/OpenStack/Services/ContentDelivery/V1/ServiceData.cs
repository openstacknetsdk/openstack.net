namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceData : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="FlavorId"/> property.
        /// </summary>
        [JsonProperty("flavor_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private FlavorId _flavorId;

        /// <summary>
        /// This is the backing field for the <see cref="Domains"/> property.
        /// </summary>
        [JsonProperty("domains", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceDomain> _domains;

        /// <summary>
        /// This is the backing field for the <see cref="Origins"/> property.
        /// </summary>
        [JsonProperty("origins", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceOrigin> _origins;

        /// <summary>
        /// This is the backing field for the <see cref="CachingRules"/> property.
        /// </summary>
        [JsonProperty("caching", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceCache> _caching;

        /// <summary>
        /// This is the backing field for the <see cref="Restrictions"/> property.
        /// </summary>
        [JsonProperty("restrictions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceRestriction> _restrictions;

        public ServiceData(string name, FlavorId flavorId, ImmutableArray<ServiceDomain> domains, ImmutableArray<ServiceOrigin> origins, ImmutableArray<ServiceCache> caching, ImmutableArray<ServiceRestriction> restrictions)
        {
            _name = name;
            _flavorId = flavorId;
            _domains = domains;
            _origins = origins;
            _caching = caching;
            _restrictions = restrictions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceData()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public FlavorId FlavorId
        {
            get
            {
                return _flavorId;
            }
        }

        public ImmutableArray<ServiceDomain> Domains
        {
            get
            {
                return _domains;
            }
        }

        public ImmutableArray<ServiceOrigin> Origins
        {
            get
            {
                return _origins;
            }
        }

        public ImmutableArray<ServiceCache> CachingRules
        {
            get
            {
                return _caching;
            }
        }

        public ImmutableArray<ServiceRestriction> Restrictions
        {
            get
            {
                return _restrictions;
            }
        }
    }
}
