namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    /// <summary>
    /// Represents a flavor resource in the Content Delivery Service.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Flavor : FlavorData
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private FlavorId _id;

        /// <summary>
        /// This is the backing field for the <see cref="Links"/> property.
        /// </summary>
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _links;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Flavor"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Flavor()
        {
        }

        /// <summary>
        /// Gets the unique ID of the flavor resource.
        /// </summary>
        /// <value>
        /// <para>The unique ID of the flavor resource.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public FlavorId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets a collection of links to resources related to the flavor resource.
        /// </summary>
        /// <value>
        /// <para>A collection of links to resources related to the flavor resource.</para>
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
