namespace OpenStack.Services.Identity
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// This class models the JSON representation of the response to an HTTP call to obtain a particular
    /// <seealso cref="ApiVersion"/> resource from a service.
    /// </summary>
    /// <seealso cref="IBaseIdentityService.PrepareGetApiVersionAsync"/>
    /// <seealso cref="BaseIdentityServiceExtensions.GetApiVersionAsync"/>
    /// <seealso cref="GetApiVersionApiCall"/>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v2.html#identity-v2-versions">API versions (Identity API v2.0 - OpenStack Complete API Reference)</seealso>
    /// <seealso href="http://developer.openstack.org/api-ref-identity-v3.html#versions-identity-v3">API versions (Identity API v3 - OpenStack Complete API Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiVersionResponse : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Version"/> property.
        /// </summary>
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiVersion _version;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersionResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ApiVersionResponse()
        {
        }

        /// <summary>
        /// Gets the <seealso cref="ApiVersion"/> resource wrapped by this object.
        /// </summary>
        /// <value>
        /// <para>The <seealso cref="ApiVersion"/> resource wrapped by this object.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ApiVersion Version
        {
            get
            {
                return _version;
            }
        }
    }
}
