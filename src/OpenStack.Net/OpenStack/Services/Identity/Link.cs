namespace OpenStack.Services.Identity
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a link, which describes an external resource related to a JSON
    /// object in some manner.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Link : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Target"/> property.
        /// </summary>
        [JsonProperty("href", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _href;

        /// <summary>
        /// This is the backing field for the <see cref="Relation"/> property.
        /// </summary>
        [JsonProperty("rel", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _rel;

        /// <summary>
        /// This is the backing field for the <see cref="Type"/> property.
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _type;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Link()
        {
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the related resource.
        /// </summary>
        /// <value>
        /// <para>The <see cref="Uri"/> of the related resource.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Uri Target
        {
            get
            {
                if (_href == null)
                    return null;

                return new Uri(_href);
            }
        }

        /// <summary>
        /// Gets a value indicating the manner in which the target resource is related to an object.
        /// </summary>
        /// <value>
        /// <para>A value indicating the manner in which the target resource is related to an object.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Relation
        {
            get
            {
                return _rel;
            }
        }

        /// <summary>
        /// Gets the HTTP media type of the related resource.
        /// </summary>
        /// <value>
        /// <para>The HTTP media type of the related resource.</para>
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
