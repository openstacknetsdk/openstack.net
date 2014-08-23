namespace OpenStack.Services.Identity.V2
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a single endpoint for an entry in the service catalog.
    /// </summary>
    /// <seealso cref="ServiceCatalogEntry.Endpoints"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Endpoint : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private EndpointId _id;

        /// <summary>
        /// This is the backing field for the <see cref="Region"/> property.
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _region;

        /// <summary>
        /// This is the backing field for the <see cref="PublicUrl"/> property.
        /// </summary>
        [JsonProperty("publicURL", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _publicUrl;

        /// <summary>
        /// This is the backing field for the <see cref="InternalUrl"/> property.
        /// </summary>
        [JsonProperty("internalURL", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _internalUrl;

        /// <summary>
        /// This is the backing field for the <see cref="AdminUrl"/> property.
        /// </summary>
        [JsonProperty("adminURL", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _adminUrl;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Endpoint()
        {
        }

        /// <summary>
        /// Gets the unique identifier for the endpoint.
        /// </summary>
        /// <value>
        /// <para>The unique identifier for the endpoint.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public EndpointId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets the region for the endpoint.
        /// </summary>
        /// <value>
        /// <para>The region for the endpoint.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Region
        {
            get
            {
                return _region;
            }
        }

        /// <summary>
        /// Gets the base URI for accessing the service from external networks.
        /// </summary>
        /// <value>
        /// <para>The base URI for accessing the service from external networks.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Uri PublicUrl
        {
            get
            {
                if (_publicUrl == null)
                    return null;

                return new Uri(_publicUrl);
            }
        }

        /// <summary>
        /// Gets the base URI for accessing the service from internal networks.
        /// </summary>
        /// <value>
        /// <para>The base URI for accessing the service from internal networks.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Uri InternalUrl
        {
            get
            {
                if (_internalUrl == null)
                    return null;

                return new Uri(_internalUrl);
            }
        }

        /// <summary>
        /// Gets the base URI for accessing the administrative API of the service.
        /// </summary>
        /// <value>
        /// <para>The base URI for accessing the administrative API of the service.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Uri AdminUrl
        {
            get
            {
                if (_adminUrl == null)
                    return null;

                return new Uri(_adminUrl);
            }
        }
    }
}
