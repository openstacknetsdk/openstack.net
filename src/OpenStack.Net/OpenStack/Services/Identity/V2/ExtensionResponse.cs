namespace OpenStack.Services.Identity.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of an HTTP API response containing an <see cref="V2.Extension"/>
    /// object.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionResponse : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Extension"/> property.
        /// </summary>
        [JsonProperty("extension", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Extension _extension;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ExtensionResponse()
        {
        }

        /// <summary>
        /// Gets the <see cref="V2.Extension"/> object.
        /// </summary>
        /// <value>
        /// <para>The <see cref="V2.Extension"/> object</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Extension Extension
        {
            get
            {
                return _extension;
            }
        }
    }
}
