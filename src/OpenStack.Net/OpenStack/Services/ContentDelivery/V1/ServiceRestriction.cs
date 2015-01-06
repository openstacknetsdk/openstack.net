namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceRestriction : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Rules"/> property.
        /// </summary>
        [JsonProperty("rules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceRestrictionRule> _rules;

        public ServiceRestriction(string name, ImmutableArray<ServiceRestrictionRule> rules)
        {
            _name = name;
            _rules = rules;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRestriction"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceRestriction()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ImmutableArray<ServiceRestrictionRule> Rules
        {
            get
            {
                return _rules;
            }
        }
    }
}
