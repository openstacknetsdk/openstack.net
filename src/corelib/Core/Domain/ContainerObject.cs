namespace net.openstack.Core.Domain
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides the details of an object stored in an Object Storage provider.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ContainerObject
    {
        /// <summary>
        /// Gets a "name" associated with the object.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/serialized-list-output.html">Serialized List Output (OpenStack Object Storage API v1 Reference)</seealso>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the "hash" value associated with the object.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/serialized-list-output.html">Serialized List Output (OpenStack Object Storage API v1 Reference)</seealso>
        [JsonProperty("hash")]
        public Guid Hash { get; set; }

        /// <summary>
        /// Gets the "bytes" value associated with the object.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/serialized-list-output.html">Serialized List Output (OpenStack Object Storage API v1 Reference)</seealso>
        [JsonProperty("bytes")]
        public long Bytes { get; set; }

        /// <summary>
        /// Gets the "content type" value associated with the object.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/serialized-list-output.html">Serialized List Output (OpenStack Object Storage API v1 Reference)</seealso>
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the "last modified" value associated with the object.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/serialized-list-output.html">Serialized List Output (OpenStack Object Storage API v1 Reference)</seealso>
        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }
    }
}
