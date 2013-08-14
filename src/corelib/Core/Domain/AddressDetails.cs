namespace net.openstack.Core.Domain
{
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a network address in a format compatible with communication with the Compute Service APIs.
    /// </summary>
    /// <seealso cref="IComputeProvider.ListAddresses"/>
    /// <seealso cref="IComputeProvider.ListAddressesByNetwork"/>
    [JsonObject(MemberSerialization.OptIn)]
    public class AddressDetails
    {
        /// <summary>
        /// Gets the network address. This is an IPv4 address if <see cref="Version"/> is <c>"4"</c>,
        /// or an IPv6 address if <see cref="Version"/> is <c>"6"</c>.
        /// </summary>
        [JsonProperty("addr")]
        public string Address { get; private set; }

        /// <summary>
        /// Gets the network address version. The value is either <c>"4"</c> or <c>"6"</c>.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; private set; }
    }
}
