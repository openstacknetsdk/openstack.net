namespace OpenStack.Services.Identity.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a role within the OpenStack Identity Service V2.
    /// </summary>
    /// <seealso cref="User.Roles"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Role : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Role()
        {
        }

        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        /// <value>
        /// <para>The name of the role.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
