namespace OpenStack.Services.Identity.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of a Tenant resource in the OpenStack Identity Service V2.
    /// </summary>
    /// <remarks>
    /// <para>This object is included with the <see cref="Token"/> returned by an authentication call, or obtained by a
    /// call to <see cref="IIdentityService.PrepareListTenantsAsync"/>.</para>
    /// </remarks>
    /// <seealso cref="Token.Tenant"/>
    /// <seealso cref="IIdentityService.PrepareListTenantsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tenant : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _id;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="Description"/> property.
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        /// <summary>
        /// This is the backing field for the <see cref="Enabled"/> property.
        /// </summary>
        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _enabled;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Tenant()
        {
        }

        /// <summary>
        /// Gets the unique ID of the tenant.
        /// </summary>
        /// <value>
        /// <para>The unique ID of the tenant.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ProjectId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets the name of the tenant.
        /// </summary>
        /// <value>
        /// <para>The name of the tenant.</para>
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
        /// Gets a description of the tenant.
        /// </summary>
        /// <value>
        /// <para>A description of the tenant.</para>
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
        /// Gets a value indicating whether the tenant is currently enabled.
        /// </summary>
        /// <value>
        /// <para><see langword="true"/> if the tenant is enabled.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the tenant is not enabled.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public bool? Enabled
        {
            get
            {
                return _enabled;
            }
        }
    }
}
