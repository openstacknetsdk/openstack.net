namespace net.openstack.Core.Domain
{
    using Newtonsoft.Json;

    /// <summary>
    /// Contains detailed information about a flavor.
    /// </summary>
    /// <seealso cref="IComputeProvider.ListFlavorsWithDetails"/>
    /// <seealso cref="IComputeProvider.GetFlavor"/>
    [JsonObject(MemberSerialization.OptIn)]
    public class FlavorDetails : Flavor
    {
        /// <summary>
        /// Gets the "OS-FLV-DISABLED:disabled" property associated with the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("OS-FLV-DISABLED:disabled")]
        public bool Disabled { get; private set; }

        /// <summary>
        /// Gets the "disk" property associated with the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("disk")] 
        public int DiskSizeInGB { get; private set; }

        /// <summary>
        /// Gets the "ram" property associated with the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("ram")]
        public int RAMInMB { get; private set; }

        //"rxtx_factor": 2.0,
 
        //"swap": 512, 

        /// <summary>
        /// Gets the "vcpus" property associated with the flavor.
        /// <note type="warning">The value of this property is not defined. Do not use.</note>
        /// </summary>
        [JsonProperty("vcpus")]
        public int VirtualCPUCount { get; private set; }
    }
}