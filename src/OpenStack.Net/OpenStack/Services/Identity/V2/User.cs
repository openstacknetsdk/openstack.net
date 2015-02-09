namespace OpenStack.Services.Identity.V2
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a User resource in the OpenStack Identity Service V2.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class User : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private UserId _id;

        /// <summary>
        /// This is the backing field for the <see cref="Username"/> property.
        /// </summary>
        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _username;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Roles"/> property.
        /// </summary>
        [JsonProperty("roles", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Role> _roles;

        /// <summary>
        /// This is the backing field for the <see cref="RolesLinks"/> property.
        /// </summary>
        [JsonProperty("roles_links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _rolesLinks;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected User()
        {
        }

        /// <summary>
        /// Gets the unique ID of the user.
        /// </summary>
        /// <value>
        /// <para>The unique Id of the user.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public UserId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets the user name of the user.
        /// </summary>
        /// <value>
        /// <para>The user name of the user.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Username
        {
            get
            {
                return _username;
            }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// <para>The name of the user.</para>
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
        /// Gets a collection of roles assigned to the user.
        /// </summary>
        /// <value>
        /// <para>A collection of roles assigned to the user.</para>
        /// <token>DefaultArrayIfNotIncluded</token>
        /// </value>
        public ImmutableArray<Role> Roles
        {
            get
            {
                return _roles;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="Link"/> objects describing resources related to the collection of
        /// <see cref="Roles"/>.
        /// </summary>
        /// <value>
        /// <para>A collection of <see cref="Link"/> objects describing resources related to the collection of
        /// <see cref="Roles"/>.</para>
        /// <token>DefaultArrayIfNotIncluded</token>
        /// </value>
        public ImmutableArray<Link> RolesLinks
        {
            get
            {
                return _rolesLinks;
            }
        }
    }
}
