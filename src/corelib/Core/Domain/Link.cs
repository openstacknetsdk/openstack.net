namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a link associated with a resource.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/LinksReferences.html">Links and References (OpenStack Compute API v2 and Extensions Reference)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    public class Link
    {
        /// <summary>
        /// Gets the link target.
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/LinksReferences.html">Links and References (OpenStack Compute API v2 and Extensions Reference)</seealso>
        [JsonProperty("href")]
        public string Href { get; private set; }

        /// <summary>
        /// Gets the link relation.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>A <c>self</c> link contains a versioned link to the resource. Use these links when the link will be followed immediately.</item>
        /// <item>A <c>bookmark</c> link provides a permanent link to a resource that is appropriate for long-term storage.</item>
        /// <item>An <c>alternate</c> link can contain an alternative representation of the resource. For example, an OpenStack Compute image might have an alternate representation in the OpenStack Image service.</item>
        /// </list>
        /// </remarks>
        /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/LinksReferences.html">Links and References (OpenStack Compute API v2 and Extensions Reference)</seealso>
        [JsonProperty("rel")]
        public string Rel { get; private set; }
    }
}
