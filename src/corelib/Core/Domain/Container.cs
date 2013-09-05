namespace net.openstack.Core.Domain
{
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the detailed information for a container stored in an Object Storage Provider.
    /// </summary>
    /// <seealso cref="IObjectStorageProvider"/>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/s_serializedlistoutput.html">Serialized List Output (OpenStack Object Storage API v1 Reference)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    public class Container
    {
        /// <summary>
        /// Gets the name of the container.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the number of objects in the container.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; private set; }

        /// <summary>
        /// Gets the total space utilized by the objects in this container.
        /// </summary>
        [JsonProperty("bytes")]
        public long Bytes { get; private set; }
    }
}
