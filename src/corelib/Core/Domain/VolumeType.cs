namespace net.openstack.Core.Domain
{
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the type of a volume in the Block Storage service.
    /// </summary>
    /// <seealso cref="IBlockStorageProvider.ListVolumeTypes"/>
    /// <seealso cref="IBlockStorageProvider.DescribeVolumeType"/>
    [JsonObject(MemberSerialization.OptIn)]
    public class VolumeType
    {
        /// <summary>
        /// Gets the volume type ID.
        /// </summary>
        [JsonProperty]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of the volume type.
        /// </summary>
        [JsonProperty]
        public string Name { get; private set; }
    }
}
