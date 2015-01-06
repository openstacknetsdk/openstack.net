namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class FlavorProvider : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Provider"/> property.
        /// </summary>
        [JsonProperty("provider", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private FlavorProviderName _provider;

        /// <summary>
        /// This is the backing field for the <see cref="Links"/> property.
        /// </summary>
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _links;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="FlavorProvider"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected FlavorProvider()
        {
        }

        public FlavorProviderName Provider
        {
            get
            {
                return _provider;
            }
        }

        /// <summary>
        /// Gets a collection of links to resources related to the provider.
        /// </summary>
        /// <value>
        /// <para>A collection of links to resources related to the provider.</para>
        /// <token>DefaultArrayIfNotIncluded</token>
        /// </value>
        public ImmutableArray<Link> Links
        {
            get
            {
                return _links;
            }
        }
    }
}
