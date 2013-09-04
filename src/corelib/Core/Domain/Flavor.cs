namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    /// <summary>
    /// Provides basic information about a flavor. A flavor is an available hardware configuration for a server.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/Flavors-d1e4180.html">Flavors (OpenStack Compute API v2 and Extensions Reference - API v2)</seealso>
    /// <seealso href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/Flavors-d1e4180.html">Flavors (Rackspace Next Generation Cloud Servers Developer Guide  - API v2)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    public class Flavor
    {
        /// <summary>
        /// Gets the unique identifier for the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the "links" property associated with the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("links")]
        public Link[] Links { get; private set; }

        /// <summary>
        /// Gets the name of the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }
    }
}
