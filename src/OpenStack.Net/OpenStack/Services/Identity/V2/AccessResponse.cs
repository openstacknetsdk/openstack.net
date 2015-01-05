namespace OpenStack.Services.Identity.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of an HTTP API response containing an <see cref="V2.Access"/> object.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class AccessResponse : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Access"/> property.
        /// </summary>
        [JsonProperty("access", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Access _access;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AccessResponse()
        {
        }

        /// <summary>
        /// Gets the <see cref="V2.Access"/> object.
        /// </summary>
        /// <value>
        /// <para>The <see cref="V2.Access"/> object</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public Access Access
        {
            get
            {
                return _access;
            }
        }
    }
}
