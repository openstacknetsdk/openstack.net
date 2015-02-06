namespace OpenStack.ObjectModel.JsonHome
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    /// <summary>
    /// This class models the authentication requirements resource hints
    /// of the home document described by <strong>Home Documents for HTTP APIs</strong>.
    /// </summary>
    /// <seealso cref="ResourceHints.AuthenticationRequirements"/>
    /// <seealso href="http://tools.ietf.org/html/draft-nottingham-json-home-03#section-4.9">Resource Hints: auth-req (Home Documents for HTTP APIs - draft-nottingham-json-home-03)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class AuthenticationRequirement : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Scheme"/> property.
        /// </summary>
        [JsonProperty("scheme", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _scheme;

        /// <summary>
        /// This is the backing field for the <see cref="Realms"/> property.
        /// </summary>
        [JsonProperty("realms", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<string> _realms;
#pragma warning restore 649

        /// <summary>
        /// Gets the HTTP authentication scheme.
        /// </summary>
        /// <value>
        /// <para>The HTTP authentication scheme.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Scheme
        {
            get
            {
                return _scheme;
            }
        }

        /// <summary>
        /// Gets an optional collection of identity protection spaces the resource is a member of.
        /// </summary>
        /// <value>
        /// <para>An collection of identity protection spaces the resource is a member of.</para>
        /// <token>DefaultArrayIfNotIncluded</token>
        /// </value>
        public ImmutableArray<string> Realms
        {
            get
            {
                return _realms;
            }
        }
    }
}
