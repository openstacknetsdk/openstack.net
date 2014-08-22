namespace OpenStack.Services.Identity
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a description of an HTTP media type supported by a particular
    /// version of an API.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class MediaType : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Base"/> property.
        /// </summary>
        [JsonProperty("base", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _base;

        /// <summary>
        /// This is the backing field for the <see cref="Type"/> property.
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _type;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaType"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MediaType()
        {
        }

        /// <summary>
        /// Gets the name of the media type describing the underlying structure of the content sent and received by the
        /// API.
        /// </summary>
        /// <value>
        /// <para>The name of the media type describing the underlying structure of the content sent and received by the
        /// API.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Base
        {
            get
            {
                return _base;
            }
        }

        /// <summary>
        /// Gets the name of the (possibly vendor-specific) media type used for requests to the API.
        /// </summary>
        /// <value>
        /// <para>The name of the (possibly vendor-specific) media type used for requests to the API.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Type
        {
            get
            {
                return _type;
            }
        }
    }
}
