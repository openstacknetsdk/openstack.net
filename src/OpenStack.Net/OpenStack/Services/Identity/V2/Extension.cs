namespace OpenStack.Services.Identity.V2
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of the description of an extension to the OpenStack Identity Service
    /// V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Extension : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Alias"/> property.
        /// </summary>
        [JsonProperty("alias", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ExtensionAlias _alias;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Namespace"/> property.
        /// </summary>
        [JsonProperty("namespace", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _namespace;

        /// <summary>
        /// This is the backing field for the <see cref="Description"/> property.
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        /// <summary>
        /// This is the backing field for the <see cref="LastModified"/> property.
        /// </summary>
        /// <remarks>
        /// <para>This is stored as a <see cref="string"/> since OpenStack currently returns the timestamp in an
        /// unrecognized format. Rather than throw an exception during initial deserialization, defer the exception to
        /// cases where the <see cref="LastModified"/> property is accessed.</para>
        /// </remarks>
        [JsonProperty("updated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _updated;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Extension"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Extension()
        {
        }

        /// <summary>
        /// Gets the unique alias identifying the extension.
        /// </summary>
        /// <value>
        /// <para>The unique alias identifying the extension.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ExtensionAlias Alias
        {
            get
            {
                return _alias;
            }
        }

        /// <summary>
        /// Gets the display name of the extension.
        /// </summary>
        /// <value>
        /// <para>The display name of the extension.</para>
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
        /// Gets the namespace in which an extension is defined.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <value>
        /// <para>The namespace in which an extension is defined.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Uri Namespace
        {
            get
            {
                if (_namespace == null)
                    return null;

                return new Uri(_namespace);
            }
        }

        /// <summary>
        /// Gets a description of the extension.
        /// </summary>
        /// <value>
        /// <para>A description of the extension.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Description
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Gets a timestamp indicating when the extension was last updated.
        /// </summary>
        /// <value>
        /// <para>A timestamp indicating when the extension was last updated.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public DateTimeOffset? LastModified
        {
            get
            {
                if (_updated == null)
                    return null;

                return JsonConvert.DeserializeObject<DateTimeOffset>(_updated);
            }
        }
    }
}
