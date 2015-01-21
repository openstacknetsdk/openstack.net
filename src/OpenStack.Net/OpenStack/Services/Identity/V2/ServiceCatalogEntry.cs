namespace OpenStack.Services.Identity.V2
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a single entry of the service catalog returned when authenticating
    /// with the Identity Service V2.
    /// </summary>
    /// <seealso cref="Access.ServiceCatalog"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceCatalogEntry : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Type"/> property.
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _type;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Endpoints"/> property.
        /// </summary>
        [JsonProperty("endpoints", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Endpoint> _endpoints;

        /// <summary>
        /// This is the backing field for the <see cref="EndpointsLinks"/> property.
        /// </summary>
        [JsonProperty("endpoints_links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _endpointsLinks;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCatalogEntry"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceCatalogEntry()
        {
        }

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>
        /// <para>The type of the service.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Gets the vendor-specific name of the service.
        /// </summary>
        /// <value>
        /// <para>The vendor-specific name of the service.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets a collection of <seealso cref="Endpoint"/> objects describing the available endpoints for the service.
        /// </summary>
        /// <value>
        /// <para>A collection of <seealso cref="Endpoint"/> objects describing the available endpoints for the
        /// service.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ImmutableArray<Endpoint> Endpoints
        {
            get
            {
                return _endpoints;
            }
        }

        /// <summary>
        /// Gets a collection of links to external resources associated with the service.
        /// </summary>
        /// <value>
        /// <para>A collection of <seealso cref="Link"/> objects describing external resources associated with the
        /// service.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ImmutableArray<Link> EndpointsLinks
        {
            get
            {
                return _endpointsLinks;
            }
        }
    }
}
