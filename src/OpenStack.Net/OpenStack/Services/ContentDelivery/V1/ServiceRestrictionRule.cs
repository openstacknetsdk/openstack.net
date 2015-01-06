namespace OpenStack.Services.ContentDelivery.V1
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceRestrictionRule : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Referrer"/> property.
        /// </summary>
        [JsonProperty("referrer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _referrer;

        public ServiceRestrictionRule(string name, string referrer)
        {
            _name = name;
            _referrer = referrer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRestrictionRule"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceRestrictionRule()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Referrer
        {
            get
            {
                return _referrer;
            }
        }
    }
}
