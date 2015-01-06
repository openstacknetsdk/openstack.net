namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class FlavorData : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Providers"/> property.
        /// </summary>
        [JsonProperty("providers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<FlavorProvider> _providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlavorData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected FlavorData()
        {
        }

        public ImmutableArray<FlavorProvider> Providers
        {
            get
            {
                return _providers;
            }
        }
    }
}
