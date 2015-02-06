namespace OpenStack.Services.Identity
{
    using System;
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of an API version for the OpenStack Identity Service.
    /// </summary>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-v2-versions">API versions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html#versions-identity-v3">API versions (Identity API v3 - OpenStack Complete API Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiVersion : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiVersionId _id;

        /// <summary>
        /// This is the backing field for the <see cref="Status"/> property.
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiStatus _status;

        /// <summary>
        /// This is the backing field for the <see cref="LastModified"/> property.
        /// </summary>
        [JsonProperty("updated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _updated;

        /// <summary>
        /// This is the backing field for the <see cref="MediaTypes"/> property.
        /// </summary>
        [JsonProperty("media-types", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<MediaType> _mediaTypes;

        /// <summary>
        /// This is the backing field for the <see cref="Links"/> property.
        /// </summary>
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _links;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersion"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ApiVersion()
        {
        }

        /// <summary>
        /// Gets the ID of the API version resource.
        /// </summary>
        /// <value>
        /// <para>The ID of the API version resource.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ApiVersionId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets the status of an exposed API.
        /// </summary>
        /// <value>
        /// <para>An <seealso cref="ApiStatus"/> instance representing the status of a particular version of an API.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ApiStatus Status
        {
            get
            {
                return _status;
            }
        }

        /// <summary>
        /// Gets a timestamp indicating when this version of a service was last updated.
        /// </summary>
        /// <value>
        /// <para>A timestamp indicating when this version of a service was last updated.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public DateTimeOffset? LastModified
        {
            get
            {
                return _updated;
            }
        }

        /// <summary>
        /// Gets a collection of <seealso cref="MediaType"/> objects describing the media types supported by this
        /// version of the API.
        /// </summary>
        /// <value>
        /// <para>A collection of <seealso cref="MediaType"/> objects describing the media types supported by this
        /// version of the API.</para>
        /// <token>DefaultArrayIfNotIncluded</token>
        /// </value>
        public ImmutableArray<MediaType> MediaTypes
        {
            get
            {
                return _mediaTypes;
            }
        }

        /// <summary>
        /// Gets a collection of <seealso cref="Link"/> objects describing external resources related to this version of
        /// the API.
        /// </summary>
        /// <value>
        /// <para>A collection of <seealso cref="Link"/> objects describing external resources related to this version of
        /// the API.</para>
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
